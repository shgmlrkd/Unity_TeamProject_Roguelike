using System;
using System.Collections;
using UnityEngine;

public class AttackMonsterState : MonsterBase
{
    private bool isAttacking = false;

    protected AnimationController controller;

    protected override void Awake()
    {
        base.Awake();
        if(controller == null )
        {
            controller = GetComponent<AnimationController>();
        }
    }
    protected virtual void OnEnable()
    {
        // 공격 할 몬스터에 따라 다르기 때문에 상속 준비
    }

    public void AttackDamage()
    {

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
        isAttacking = false;
        monsterStateManager.SetState(MonsterStateEnum.Idle);
    }
    // 애니메이션 프레임으로 이벤트 추가로 공격이 되게 연결 할 예정
    // 코루틴으로 안해도 충분하다
}