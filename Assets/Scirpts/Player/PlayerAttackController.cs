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

    private CharacterInputManager inputManager;
    private CharacterController2D characterController;
    private PlayerWeaponController weaponController;
    //공격 끝 조정
    private bool isAttacking;
    //공격중 방향 고정
    private Vector2 lockedAttackDirection = Vector2.down;

    private void Awake()
    {
        inputManager = GetComponent<CharacterInputManager>();
        characterController = GetComponent<CharacterController2D>();
        weaponController = GetComponent<PlayerWeaponController>();

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

        if (animator == null)
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

        attackAreaTransform.localPosition = attackDirection * currentWeapon.AttackDistance;

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

        DamageInfoSet damageInfo = new DamageInfoSet(
            currentWeapon.Damage,
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
}