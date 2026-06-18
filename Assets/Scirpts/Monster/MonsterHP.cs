using UnityEngine;

public class MonsterHP : MonoBehaviour, IDamageable
{

    private MonsterData monsterData;
    private MonsterStateManager monsterStateManager;
    protected int currentHp;
    
    private void Awake()
    {
        currentHp = monsterData.MonsterMaxHp;

    }

    public void TakeDamage(DamageInfoSet damageInfoset) // 받는 공격 데미지
    {
        // DamageInfoSet 의
        currentHp -= damageInfoset.Damage;
        GameObject attacker = damageInfoset.Attacker;
        Vector2 Direction = damageInfoset.AttackDirection;

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
            monsterStateManager.SetState(MonsterStateEnum.Dead);
        }
    }

    public bool IsDead
    {
        get { return currentHp <= 0; }
    }

    public void Die()
    {
        // 풀링
        Destroy(gameObject);

    }
    

    
}
