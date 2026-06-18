using System.Collections;
using UnityEngine;

public class AttackMonsterState : MonsterBase
{
    
    public void AttackDamage()
    {
        if (monsterStateManager.Target == null) return;

        if (monsterStateManager.Target.TryGetComponent(out IDamageable damageable))
        {
            DamageInfoSet damageInfoSet = new DamageInfoSet(); // 구조체에 있는 방식으로 전달
            damageInfoSet.Damage = monsterStateManager.MonsterData.AttackDamage;
            damageInfoSet.Attacker = gameObject;

            damageable.TakeDamage(damageInfoSet);
            print("공격중");
        }
    }
    // 애니메이션 프레임으로 이벤트 추가로 공격이 되게 연결 할 예정
    // 코루틴으로 안해도 충분하다
}