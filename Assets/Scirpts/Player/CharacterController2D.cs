using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Weapon")]
    [SerializeField] private WeaponData defaultWeapon;
    //현재 보유 무기 리스트로 관리
    [SerializeField] private List<WeaponData> ownedWeapons = new List<WeaponData>();
    [SerializeField] private Transform attackSpawnPoint;

    private Rigidbody2D rb;
    private CharacterInputManager inputManager;

    private Camera mainCamera;
    private Vector2 lookDirection = Vector2.down;

    private Vector2 lastMoveDirection = Vector2.down;
    private int currentWeaponIndex;
    private float nextAttackTime;

    private void Awake()
    {
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<CharacterInputManager>();

        if (attackSpawnPoint == null)
        {
            attackSpawnPoint = transform;
        }

        if (ownedWeapons.Count == 0 && defaultWeapon != null)
        {
            ownedWeapons.Add(defaultWeapon);
        }
    }

    private void OnEnable()
    {
        inputManager.OnAttackPressed += HandleAttackPressed;
        inputManager.OnWeaponSelected += ChangeWeapon;
    }

    private void OnDisable()
    {
        inputManager.OnAttackPressed -= HandleAttackPressed;
        inputManager.OnWeaponSelected -= ChangeWeapon;
    }

    private void Update()
    {
        UpdateLookDirection();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.linearVelocity = inputManager.MoveInput * moveSpeed;
    }

    private void UpdateLookDirection()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(inputManager.MousePosition);
        Vector2 direction = mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        lookDirection = direction.normalized;
    }
    //공격 버튼 누를 시 쿨타임, 현재 웨폰 검사
    private void HandleAttackPressed()
    {
        WeaponData currentWeapon = GetCurrentWeapon();

        if (currentWeapon == null)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        Attack(currentWeapon);
    }

    private void Attack(WeaponData weapon)
    {
        if (weapon.AttackPrefab == null)
        {
            Debug.LogWarning("공격 프리팹이 없습니다.");
            return;
        }

        nextAttackTime = Time.time + weapon.AttackCooldown;
        //공격 방향, 공격 정보 불러와 게임오브젝트에 저장
        GameObject attackObject = Instantiate(
            weapon.AttackPrefab,
            attackSpawnPoint.position,
            Quaternion.identity
        );

        DamageInfoSet damageInfo = new DamageInfoSet(
            weapon.Damage,
            gameObject,
            lookDirection
        );
        //AttackCollider 컴포넌트 불러와 damageInfo 정보 획득
        if (attackObject.TryGetComponent(out AttackCollider attackCollider))
        {
            attackCollider.Init(damageInfo);
        }
        //public void Init(DamageInfoSet damageInfo)
        //{
        //    this.damageInfo = damageInfo;
        //    isInitialized = true;

        //    Destroy(gameObject, lifeTime);
        //}
    }

    private void ChangeWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= ownedWeapons.Count)
        {
            return;
        }

        currentWeaponIndex = weaponIndex;

        Debug.Log($"무기 변경: {ownedWeapons[currentWeaponIndex].WeaponName}");
    }

    public void AddWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            return;
        }

        ownedWeapons.Add(weapon);
    }

    private WeaponData GetCurrentWeapon()
    {
        if (ownedWeapons.Count == 0)
        {
            return null;
        }

        if (currentWeaponIndex < 0 || currentWeaponIndex >= ownedWeapons.Count)
        {
            currentWeaponIndex = 0;
        }

        return ownedWeapons[currentWeaponIndex];
    }
}