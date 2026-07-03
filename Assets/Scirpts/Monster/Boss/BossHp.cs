using System;
using UnityEngine;

public class BossHp : MonoBehaviour, IDamageable
{
    private const float PHASE_TWO_HEAL_RATIO = 0.6f;
    private const float PHASE_TWO_THRESHOLD = 0.25f;

    private BossStateManager stateManager;

    private int currentHp;
    private int phaseTwoHp;

    private bool isInvincible = false;
    private bool hasEnteredPhaseTwo = false;

    public event Action<int, int> OnBossHit;
    public event Action<int, int> OnBossHeal;
    public event Action OnBossDead;

    public bool IsDead => currentHp <= 0;

    private void Awake()
    {
        stateManager = GetComponent<BossStateManager>();
    }

    private void Start()
    {
        currentHp = stateManager.Context.data.MaxHp;

        phaseTwoHp = (int)(stateManager.Context.data.MaxHp * PHASE_TWO_THRESHOLD);

        stateManager.Context.OnHealRequested += HealForPhaseTwo;
        stateManager.Context.OnInvincibleChanged += SetInvincible;
    }

    private void OnDisable()
    {
        stateManager.Context.OnHealRequested -= HealForPhaseTwo;
        stateManager.Context.OnInvincibleChanged -= SetInvincible;
    }

    private void HealForPhaseTwo()
    {
        int maxHp = stateManager.Context.data.MaxHp;
        int healAmount = (int)(maxHp * PHASE_TWO_HEAL_RATIO);

        currentHp = healAmount;

        OnBossHeal?.Invoke(healAmount, maxHp);
    }

    private void CheckPhaseTwo()
    {
        if (!hasEnteredPhaseTwo && currentHp <= phaseTwoHp)
        {
            hasEnteredPhaseTwo = true;
            stateManager.Context.EnterPhaseTwo();
        }
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public void TakeDamage(DamageInfoSet damageInfoset)
    {
        if (isInvincible)
        {
            print("무적");
            return;
        }

        currentHp -= damageInfoset.Damage;

        CheckPhaseTwo();

        OnBossHit?.Invoke(currentHp, stateManager.Context.data.MaxHp);

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
            return;
        }

        stateManager.VisualController.PlayHitFlash();
        SoundManager.Instance.PlaySFX(SoundKey.MonsterHit);
    }

    public void Die()
    {
        InGameManager.Instance.RegisterBossKill();
        OnBossDead?.Invoke();
        stateManager.SetState(BossStateEnum.Dead);
    }
}
