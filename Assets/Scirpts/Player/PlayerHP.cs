using System;
using System.Collections;
using UnityEngine;

public class PlayerHP : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D bodyCollider;

    [SerializeField] private int maxHp = 10;
    [SerializeField] private const int MaxBonusHp = 20;


    [SerializeField] private float invincibleTime = 0.8f;

    [Header("Death Fade")]
    [SerializeField] private float fadeOutDuration = 1.0f;
    [SerializeField] private float disableAlphaThreshold = 0.02f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalSpriteColors;
    private Coroutine fadeOutCoroutine;

    private int currentHp;
    // 추가 체력 저장할 변수
    private int currentBonusHp = 0;

    private bool isDead;

    private bool isInvincible;
    private Coroutine invincibleCoroutine;

    private PlayerInventory playerInventory;
    private int previousEquipmentShieldHp;

    private CharacterController2D characterController;
    private PlayerAttackController attackController;
    private CharacterInputManager inputManager;
    private Rigidbody2D rb;
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
            animator = GetComponentInChildren<Animator>();
        }
        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<BoxCollider2D>();
        }

        characterController = GetComponent<CharacterController2D>();
        attackController = GetComponent<PlayerAttackController>();
        inputManager = GetComponent<CharacterInputManager>();
        rb = GetComponent<Rigidbody2D>();
        playerInventory = GetComponent<PlayerInventory>();

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalSpriteColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalSpriteColors[i] = spriteRenderers[i].color;
        }
    }

    private void Start()
    {
        OnHpChanged?.Invoke(currentHp, currentBonusHp);
    }

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnBonusStatChanged += HandleBonusStatChanged;
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnBonusStatChanged -= HandleBonusStatChanged;
        }
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

    //최대체력 2의 배수 닐시 1더함
    public void SetMaxHp(int newMaxHp)
    {
        if (isDead)
        {
            return;
        }

        newMaxHp = Mathf.Max(2, newMaxHp);

        if (newMaxHp % 2 != 0)
        {
            newMaxHp += 1;
        }

        maxHp = newMaxHp;
        currentHp = maxHp;

        OnHpChanged?.Invoke(currentHp, currentBonusHp);
    }
    
    //추가체력 변동 대응
    private void HandleBonusStatChanged(BonusStat bonusStat)
    {
        if (bonusStat == null || isDead)
        {
            return;
        }

        currentBonusHp = bonusStat.ShieldHp;
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

            if (currentBonusHp <= 0)// 추가 체력으로 막지 못한 남은 데미지
            {
                int overflowDamage = -currentBonusHp;
                currentHp -= overflowDamage;
                currentBonusHp = 0;
                playerInventory.RemoveEquipment(EquipmentType.Shield);
            }
            else
            {
                playerInventory.DamageShield(damageInfoset.Damage); // 방패가 살아있을 때만 내구도 갱신
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
        if (attackController != null)
        {
            attackController.CancelAttack();
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

        if (attackController != null)
        {
            attackController.CancelAttack();
        }

        DisableControl();
        
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            //애니메이터 오류발생대응
            StartFadeOut();
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
        StartFadeOut();
    }

    private void StartFadeOut()
    {
        if (fadeOutCoroutine != null)
        {
            return;
        }

        fadeOutCoroutine = StartCoroutine(FadeOutAndDisableCo());
    }

    private IEnumerator FadeOutAndDisableCo()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            DisablePlayer();
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / fadeOutDuration);
            float alphaMultiplier = Mathf.Lerp(1f, 0f, t);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                {
                    continue;
                }

                Color color = originalSpriteColors[i];
                color.a = originalSpriteColors[i].a * alphaMultiplier;
                spriteRenderers[i].color = color;
            }

            if (alphaMultiplier <= disableAlphaThreshold)
            {
                break;
            }

            yield return null;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }

            Color color = spriteRenderers[i].color;
            color.a = 0f;
            spriteRenderers[i].color = color;
        }

        DisablePlayer();
    }

    private void DisablePlayer()
    {
        gameObject.SetActive(false);
    }

    private void DisableControl()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }


        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        if (attackController != null)
        {
            attackController.enabled = false;
        }

        if (inputManager != null)
        {
            inputManager.enabled = false;
        }
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