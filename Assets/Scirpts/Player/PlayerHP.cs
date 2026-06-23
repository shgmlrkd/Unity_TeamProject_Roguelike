using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHP : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator animator;

    [SerializeField] private int maxHp = 10;
    [SerializeField] private const int MaxBonusHp = 20;


    [SerializeField] private float invincibleTime = 0.8f;
    private int currentHp;
    // 추가 체력 저장할 변수
    private int currentBonusHp = 0;

    private bool isDead;

    private bool isInvincible;
    private Coroutine invincibleCoroutine;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public int CurrentBonusHp => currentBonusHp;
    public bool IsDead => isDead;

    public event Action<int, int> OnHpChanged;

    //현재 체력 갱신 테스트
    //[Header("Test")]
    //[SerializeField] private int testCurrentHp = -1;

    //private int previousTestCurrentHp = -1;

    private void Awake()
    {
        currentHp = maxHp;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        OnHpChanged?.Invoke(currentHp, currentBonusHp);
    }

    // 이건 일반 체력용 호출 함수 (포션 먹기)
    public void Heal(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHp = Mathf.Clamp(currentHp + amount, 0, MaxHp);

        OnHpChanged?.Invoke(currentHp, currentBonusHp);
    }

    // 이건 추가 체력용 호출 함수
    public void SetBonusHp(int bonusHp)
    {
        if (isDead)
        {
            return;
        }

        currentBonusHp = Mathf.Clamp(currentBonusHp + bonusHp, 0, MaxBonusHp);

        OnHpChanged?.Invoke(currentHp, currentBonusHp);
    }

    public void TakeDamage(DamageInfoSet damageInfoset) // 받는 공격 데미지
    {
        if (isDead)
        {
            return;
        }
        if (isInvincible)
        {
            return;
        }

        // 현재 추가 체력이 있는가
        if (currentBonusHp > 0)
        {
            currentBonusHp -= damageInfoset.Damage;

            if (currentBonusHp < 0)// 추가 체력으로 막지 못한 남은 데미지
            {
                int overflowDamage = -currentBonusHp;
                currentHp -= overflowDamage;
                currentBonusHp = 0;
            }
        }
        else // 없으면 현재 체력에서 데미지 받음
        {
            currentHp -= damageInfoset.Damage;
        }

        GameObject attacker = damageInfoset.Attacker;
        Vector2 Direction = damageInfoset.AttackDirection;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        OnHpChanged?.Invoke(currentHp, currentBonusHp);

        if (currentHp <= 0)
        {
            currentHp = 0;

            Die();
            return;
        }
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        StartInvincible();

    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            //애니메이터 오류발생대응
            Destroy(gameObject);
        }
    }
    //피격시 무적 설정
    private void StartInvincible()
    {
        if (invincibleCoroutine != null)
        {
            StopCoroutine(invincibleCoroutine);
        }

        invincibleCoroutine = StartCoroutine(InvincibleCo());
    }
    private IEnumerator InvincibleCo()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        invincibleCoroutine = null;
    }
    //사망 애니메이션 마지막 프레임에 호출
    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }
    //private void UpdateTestCurrentHp()
    //{
    //    if (testCurrentHp == previousTestCurrentHp)
    //    {
    //        return;
    //    }

    //    previousTestCurrentHp = testCurrentHp;

    //    if (testCurrentHp < 0)
    //    {
    //        return;
    //    }

    //    currentHp = Mathf.Clamp(testCurrentHp, 0, maxHp);
    //    isDead = currentHp <= 0;

    //    OnHpChanged?.Invoke(currentHp, maxHp);
    //}
}