using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private AttackCollider attackCollider;
    [SerializeField] private Collider2D attackHitbox;
    [SerializeField] private Transform attackAreaTransform;
    [SerializeField] private BoxCollider2D attackBoxCollider;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Attack VFX")]
    [SerializeField] private GameObject attackVfxPrefab;
    [SerializeField] private float attackVfxLifeTime = 0.35f;
    [SerializeField] private float attackVfxAngleOffset = 0f;
    [SerializeField] private Vector3 attackVfxPositionOffset;

    private CharacterInputManager inputManager;
    private CharacterController2D characterController;
    private PlayerWeaponController weaponController;
    private PlayerInventory playerInventory;
    private PlayerStartStatBonus startStatBonus;
    //공격 끝 조정
    private bool isAttacking;

    public bool IsAttacking => isAttacking;
    //공격중 방향 고정
    private Vector2 lockedAttackDirection = Vector2.down;



    private void Awake()
    {
        inputManager = GetComponent<CharacterInputManager>();
        characterController = GetComponent<CharacterController2D>();
        weaponController = GetComponent<PlayerWeaponController>();
        playerInventory = GetComponent<PlayerInventory>();
        startStatBonus = GetComponent<PlayerStartStatBonus>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        ApplyWeaponAttackSetting();
    }

    private void OnEnable()
    {
        if (inputManager != null)
        {
            inputManager.OnAttackPressed += HandleAttackPressed;
        }   

        if (weaponController != null)
        {
            weaponController.OnWeaponChanged += HandleWeaponChanged;
        }
    }

    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnAttackPressed -= HandleAttackPressed;
        }

        if (weaponController != null)
        {
            weaponController.OnWeaponChanged -= HandleWeaponChanged;
        }

        isAttacking = false;
        DisableAttackCollider();
    }

    private void Update()
    {
        if (isAttacking)
        {
            return;
        }
        UpdateAttackAreaTransform();
    }
    //공격 키 누를 시 어택 애니메이션 호출
    private void HandleAttackPressed()
    {
        if (isAttacking)
        {
            return;
        }
        if (weaponController == null || weaponController.CurrentWeapon == null)
        {
            return;
        }

        if (animator == null || characterController == null)
        {
            return;
        }

        // 공격 시 마우스 방향 저장
        lockedAttackDirection = characterController.LookDirection;

        if (lockedAttackDirection.sqrMagnitude <= 0.001f)
        {
            lockedAttackDirection = Vector2.down;
        }

        lockedAttackDirection.Normalize();

        // 저장방향으로 AttackArea를 배치
        UpdateAttackAreaTransform(lockedAttackDirection);
        //공격 속도
        ApplyAttackSpeedToAnimator();

        isAttacking = true;
        animator.SetTrigger("Attack");
    }
    private void UpdateAttackAreaTransform()
    {
        if (characterController == null)
        {
            return;
        }

        UpdateAttackAreaTransform(characterController.LookDirection);
    }
    private void UpdateAttackAreaTransform(Vector2 attackDirection)
    {
        if (attackAreaTransform == null || weaponController == null)
        {
            return;
        }

        WeaponData currentWeapon = weaponController.CurrentWeapon;

        if (currentWeapon == null)
        {
            return;
        }

        if (attackDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        attackDirection.Normalize();

        float finalAttackDistance = GetFinalAttackDistance(currentWeapon);

        attackAreaTransform.localPosition = attackDirection * finalAttackDistance;

        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        attackAreaTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    //애니메이터에 삽입
    public void EnableAttackCollider()
    {
        if (weaponController == null || characterController == null)
        {
            return;
        }

        WeaponData currentWeapon = weaponController.CurrentWeapon;

        if (currentWeapon == null || attackCollider == null || attackHitbox == null)
        {
            return;
        }
        //공격시 방향 저장
        Vector2 attackDirection = isAttacking ? lockedAttackDirection : characterController.LookDirection;

        SpawnAttackVfx(attackDirection);

        int finalDamage = GetFinalAttackDamage(currentWeapon);

        DamageInfoSet damageInfo = new DamageInfoSet(
            finalDamage,
            gameObject,
            attackDirection
        );

        attackCollider.Init(damageInfo);
        attackHitbox.enabled = true;
    }

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
    public void EndAttack()
    {
        Debug.Log("EndAttack 호출됨");
        isAttacking = false;

        UpdateAttackAreaTransform();
    }
    public void CancelAttack()
    {
        if (!isAttacking)
        {
            return;
        }

        isAttacking = false;

        DisableAttackCollider();

        UpdateAttackAreaTransform();
    }
    //
    private void HandleWeaponChanged(WeaponData weaponData)
    {
        ApplyWeaponAttackSetting();
    }
    //무기 데이터에서 정보 뽑아 적용
    private void ApplyWeaponAttackSetting()
    {
        if (weaponController == null || attackBoxCollider == null)
        {
            return;
        }

        WeaponData currentWeapon = weaponController.CurrentWeapon;

        if (currentWeapon == null)
        {
            return;
        }

        attackBoxCollider.size = currentWeapon.AttackBoxSize;
    }
    //최종 공격력, 범위, 공격 속도 추가
    private int GetFinalAttackDamage(WeaponData currentWeapon)
    {
        int finalAttack = 0;

        if (playerInventory != null && playerInventory.CurrentBonusStat != null)
        {
            finalAttack += playerInventory.CurrentBonusStat.Attack;
        }

        if (startStatBonus != null)
        {
            finalAttack += startStatBonus.Attack;
        }

        if (finalAttack > 0)
        {
            return finalAttack;
        }

        if (currentWeapon != null)
        {
            return Mathf.Max(1, currentWeapon.Damage);
        }

        return 1;
    }
    private float GetFinalAttackDistance(WeaponData currentWeapon)
    {
        if (playerInventory != null && playerInventory.CurrentBonusStat != null)
        {
            float finalRange = playerInventory.CurrentBonusStat.AttackRange;

            if (finalRange > 0f)
            {
                return finalRange;
            }
        }

        if (currentWeapon != null)
        {
            return Mathf.Max(0.1f, currentWeapon.AttackDistance);
        }

        return 0.7f;
    }
    private float GetAttackSpeedMultiplier()
    {
        float attackSpeedRate = 0f;

        if (playerInventory != null && playerInventory.CurrentBonusStat != null)
        {
            attackSpeedRate += playerInventory.CurrentBonusStat.AttackSpeedRate;
        }

        if (startStatBonus != null)
        {
            attackSpeedRate += startStatBonus.AttackSpeedRate;
        }

        return Mathf.Max(0.1f, 1f + attackSpeedRate);
    }

    private void ApplyAttackSpeedToAnimator()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("AttackSpeedMultiplier", GetAttackSpeedMultiplier());
    }
    //VFX생성
    private void SpawnAttackVfx(Vector2 attackDirection)
    {
        if (attackVfxPrefab == null || attackAreaTransform == null)
        {
            return;
        }

        if (attackDirection.sqrMagnitude <= 0.001f)
        {
            attackDirection = Vector2.down;
        }
        //공격방향 정규화
        attackDirection.Normalize();
        //방향을 각도로
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle + attackVfxAngleOffset);

        Vector3 spawnPosition = attackAreaTransform.position + attackVfxPositionOffset;

        GameObject vfxObject = Instantiate(
            attackVfxPrefab,
            spawnPosition,
            rotation
        );
        //공격속도 맞게
        float attackSpeedMultiplier = GetAttackSpeedMultiplier();

        if (vfxObject.TryGetComponent(out Animator vfxAnimator))
        {
            vfxAnimator.speed = attackSpeedMultiplier;
        }
        else
        {
            Animator childAnimator = vfxObject.GetComponentInChildren<Animator>();

            if (childAnimator != null)
            {
                childAnimator.speed = attackSpeedMultiplier;
            }
        }

        float adjustedLifeTime = attackVfxLifeTime / attackSpeedMultiplier;

        Destroy(vfxObject, adjustedLifeTime);
    }
}
