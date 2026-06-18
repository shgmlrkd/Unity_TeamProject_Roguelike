using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Weapon")]
    [SerializeField] private WeaponData defaultWeapon;
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private float weaponDropDistance = 0.8f;
    [SerializeField] private float droppedWeaponPickupDelay = 2.0f;

    [Header("Attack")]
    [SerializeField] private AttackCollider attackCollider;
    [SerializeField] private Collider2D attackHitbox;
    [SerializeField] private Transform attackAreaTransform;
    [SerializeField] private BoxCollider2D attackBoxCollider;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private CharacterInputManager inputManager;
    //화면 기준 마우스 위치 좌표 도출용
    private Camera mainCamera;

    private Vector2 lookDirection = Vector2.down;
    //[Header("Debug")]
    //[SerializeField] private LineRenderer lookDebugLine;
    //[SerializeField] private float lookDebugLineLength = 2f;

    private void Awake()
    {
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<CharacterInputManager>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (currentWeapon == null)
        {
            currentWeapon = defaultWeapon;
        }

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        ApplyWeaponAttackSetting();
    }

    private void OnEnable()
    {
        inputManager.OnAttackPressed += HandleAttackPressed;
    }

    private void OnDisable()
    {
        inputManager.OnAttackPressed -= HandleAttackPressed;
    }

    private void Update()
    {
        UpdateLookDirection();
        UpdateAttackAreaTransform();
        UpdateSpriteFlip();

        //UpdateLookDebugLine();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.linearVelocity = inputManager.MoveInput * moveSpeed;
    }
    //바라보는 방향 디버깅 용 주석처리
    //private void UpdateLookDebugLine()
    //{
    //    if (lookDebugLine == null)
    //    {   
    //        return;
    //    }

    //    lookDebugLine.positionCount = 2;
    //    lookDebugLine.SetPosition(0, transform.position);
    //    lookDebugLine.SetPosition(1, transform.position + (Vector3)(lookDirection * lookDebugLineLength));
    //}
    private void UpdateLookDirection()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseScreenPosition = inputManager.MousePosition;
        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        lookDirection = direction.normalized;
    }
    //무기 공격 범위 참조해 반영
    private void UpdateAttackAreaTransform()
    {
        if (attackAreaTransform == null || currentWeapon == null)
        {
            return;
        }

        attackAreaTransform.localPosition = lookDirection * currentWeapon.AttackDistance;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        attackAreaTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    //스프라이트 플립 바라보는 방향 따라
    private void UpdateSpriteFlip()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (lookDirection.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (lookDirection.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
    //공격 버튼 누를 시 쿨타임, 현재 웨폰 검사
    private void HandleAttackPressed()
    {
        if (currentWeapon == null)
        {
            return;
        }

        animator.SetTrigger("Attack");
    }
    //애니메이션 공격 판정 시작
    public void EnableAttackCollider()
    {
        if (currentWeapon == null || attackCollider == null || attackHitbox == null)
        {
            return;
        }

        DamageInfoSet damageInfo = new DamageInfoSet(
            currentWeapon.Damage,
            gameObject,
            lookDirection
        );

        attackCollider.Init(damageInfo);
        attackHitbox.enabled = true;
    }
    //종료
    public void DisableAttackCollider()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        if (attackCollider != null)
        {
            attackCollider.Clear();
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null)
        {
            return;
        }


        DropCurrentWeapon();

        currentWeapon = newWeapon;
        ApplyWeaponAttackSetting();

        Debug.Log($"무기 장착: {currentWeapon.WeaponName}");
    }
    //무기 드랍(새로운 무기 획득)시 기존 무기 떨어트림
    private void DropCurrentWeapon()
    {
        if (currentWeapon == null || currentWeapon.WeaponItemPrefab == null)
        {
            return;
        }

        if (currentWeapon.WeaponItemPrefab == null)
        {
            return;
        }

        Vector2 dropDirection = -lookDirection;

        if (dropDirection == Vector2.zero)
        {
            dropDirection = Vector2.down;
        }

        Vector3 dropPosition = transform.position + (Vector3)(dropDirection.normalized * weaponDropDistance);

        GameObject droppedWeapon = Instantiate(
            currentWeapon.WeaponItemPrefab,
            dropPosition,
            Quaternion.identity
        );

        if (droppedWeapon.TryGetComponent(out WeaponItem weaponItem))
        {
            weaponItem.SetPickupDelay(droppedWeaponPickupDelay);
        }
    }
    //무기 공격 범위 만큼 공격 범위 변경
    private void ApplyWeaponAttackSetting()
    {
        if (currentWeapon == null || attackBoxCollider == null)
        {
            return;
        }

        attackBoxCollider.size = currentWeapon.AttackBoxSize;
    }
}