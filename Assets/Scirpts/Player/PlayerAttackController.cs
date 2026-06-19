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

    private bool isAttacking;

    private void Awake()
    {
        inputManager = GetComponent<CharacterInputManager>();
        characterController = GetComponent<CharacterController2D>();
        weaponController = GetComponent<PlayerWeaponController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
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
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    private void UpdateAttackAreaTransform()
    {
        if (attackAreaTransform == null || characterController == null)
        {
            return;
        }

        WeaponData currentWeapon = weaponController.CurrentWeapon;

        if (currentWeapon == null)
        {
            return;
        }

        Vector2 lookDirection = characterController.LookDirection;

        attackAreaTransform.localPosition = lookDirection * currentWeapon.AttackDistance;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
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

        DamageInfoSet damageInfo = new DamageInfoSet(
            currentWeapon.Damage,
            gameObject,
            characterController.LookDirection
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