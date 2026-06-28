
public class AttackMonsterState : MonsterBase
{
    private bool isAttacking = false;


    // 공격 할 몬스터에 따라 다르기 때문에 상속 준비  

    public void AttackDamage()
    {
        // 이미 공격 상태가 아니면 데미지를 주지 않음
        // 죽었거나 다른 상태로 넘어간 뒤에 공격 이벤트가 늦게 들어오는 상황 방지
        if (monsterStateManager.CurrentState != MonsterStateEnum.Attack)
        {
            return;
        }

        if (monsterStateManager.Target == null) return;

        if (monsterStateManager.Target.TryGetComponent(out IDamageable damageable))
        {
            isAttacking = true;

            DamageInfoSet damageInfoSet = new DamageInfoSet(); // 구조체에 있는 방식으로 전달

            damageInfoSet.Damage = monsterStateManager.MonsterData.AttackDamage;

            damageInfoSet.Attacker = gameObject;

            damageable.TakeDamage(damageInfoSet);

            print("공격명중");
        }
    
    }   

    public void AnimEventBaseAttackDamage() // 애니메이션의 실제 타격 프레임에 이벤트로 연결
    {
        AttackDamage();
    }

    public void AnimEventEndAttack() // 공격 애니메이션 마지막 프레임 이벤트로 연결
    {
        // 공격 상태일 때 공격 애니메이션이 정상적으로 끝난 경우에만 Idle로 보냄
        // 이미 Dead 상태거나 다른 상태로 넘어간 뒤라면 이 이벤트는 무시함
        if (monsterStateManager.CurrentState != MonsterStateEnum.Attack)
        {
            return;
        }

        isAttacking = false;

        // 공격 후 잠깐 텀을 주기 위해 Idle 상태로 전환
        monsterStateManager.SetState(MonsterStateEnum.Idle);
    }
    
}