using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterHP : MonoBehaviour, IDamageable
{
    private MonsterData monsterData;
    private MonsterStateManager monsterStateManager;
    
    protected int currentHp;

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
    private void OnDisable()
    {
        // 죽기전에 해야할걸 하는곳
    }
    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Die();
        }
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
        }
    }

    

    public void Die()
    {
        // 예시로 몬스터가 죽었을 때
        ItemManager.Instance.DropItem(transform.position);

        monsterStateManager.SetState(MonsterStateEnum.Dead);

    }
    

    
}
