using UnityEngine;

public class BossChaseState : BossBase
{
    private const float CHASE_TIME = 3.5f;
    private const float ATTACKABLE_TIME = 1.0f;

    public override void Tick()
    {
        // 추격 시간 내에 AttackSelect 상태로 못가면 Idle 상태
        if (IsChaseTimeOver())
        {
            ChangeState(BossStateEnum.Idle);
        }
        // 추격 시간 내에 AttackSelect 상태로 가는 조건이 된다면
        else if (CanEnterAttackSelect())
        {
            ChangeState(BossStateEnum.AttackSelect);
        }
    }

    public override void FixedTick()
    {
        Vector2 current = bossContext.rb.position;
        Vector2 target = bossContext.target.position;

        Vector2 next =
            Vector2.MoveTowards(current, target, bossContext.CurrentMoveSpeed * Time.fixedDeltaTime);

        bossContext.rb.MovePosition(next);
    }

    // 추격 시간 오버
    private bool IsChaseTimeOver()
    {
        return stateTime > CHASE_TIME;
    }

    // 추격 시간 내에 타겟이 AttackSelect 범위 내에 들어오고 공격 쿨타임이 돌았다면
    private bool CanEnterAttackSelect()
    {
        bool canAttack = GetDistance() <= bossContext.data.AttackSelectRange;
        bool attackCoolTime = stateTime >= ATTACKABLE_TIME;

        return attackCoolTime && canAttack;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bossContext.data.AttackSelectRange);
    }
}