using System;
using UnityEngine;

public class MonsterHP : MonoBehaviour, IDamageable
{
    private MonsterData monsterData;
    private MonsterStateManager monsterStateManager;
    
    protected int currentHp;

    public event Action<GameObject> OnMonsterDied;

    public bool IsDead
    {
        get { return currentHp <= 0; }
    }

    private void Awake()
    {
        monsterStateManager = GetComponent<MonsterStateManager>();
        monsterData = monsterStateManager.MonsterData;
    }
    private void OnEnable()
    {
        currentHp = monsterData.MonsterMaxHp;
    }

    public void TakeDamage(DamageInfoSet damageInfoset) // 받는 공격 데미지
    {
        // DamageInfoSet 의
        currentHp -= damageInfoset.Damage;
        GameObject attacker = damageInfoset.Attacker;
        Vector2 Direction = damageInfoset.AttackDirection;

        print($"{currentHp} / {monsterData.MonsterMaxHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
            return;
        }

        SoundManager.Instance.PlaySFX(SoundKey.MonsterHit);
        monsterStateManager.SetState(MonsterStateEnum.Hit);
    }

    public void Die()
    {
        SoundManager.Instance.PlaySFX(SoundKey.MonsterDead);
        InGameManager.Instance.RegisterMonsterKill();

        OnMonsterDied?.Invoke(gameObject);
        monsterStateManager.SetState(MonsterStateEnum.Dead);
    }

}
