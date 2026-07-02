using UnityEngine;

public class BossBaseAttackState : BossBase
{
    private const float BASE_ATTACK_RANGE = 3.0f;

    private LayerMask targetLayer;

    private bool isAttacking;
    private bool isAttackEnd;

    private void Start()
    {
        targetLayer = LayerMask.GetMask("Player");
    }

    public override void Enter()
    {
        base.Enter();

        isAttacking = false;
        isAttackEnd = false;
    }

    public override void Tick()
    {
        // 공격 끝
        if (isAttackEnd)
        {
            ChangeState(BossStateEnum.Chase);
            return;
        }

        // 이미 공격 시작했으면 기다림
        if (isAttacking)
        {
            return;
        }

        // 플레이어가 사거리 밖이면 바로 추적
        if (!IsPlayerInRange())
        {
            ChangeState(BossStateEnum.Chase);
            return;
        }

        // 공격 시작
        isAttacking = true;
        bossContext.animController.OnBossBaseAttackTrigger();
    }

    private bool IsPlayerInRange()
    {
        Collider2D target = Physics2D.OverlapCircle(
            bossContext.attackPos.position,
            BASE_ATTACK_RANGE,
            targetLayer);

        return target != null;
    }

    private void AnimEventBaseAttack()
    {
        Collider2D target = Physics2D.OverlapCircle(
            bossContext.attackPos.position,
            BASE_ATTACK_RANGE,
            targetLayer);

        if (target != null && target.TryGetComponent(out IDamageable player))
        {
            DamageInfoSet damage = new DamageInfoSet(bossContext.CurrentAttackDamage);
            player.TakeDamage(damage);
        }

        isAttackEnd = true;
    }

    private void OnDrawGizmos()
    {
        if (bossContext == null || bossContext.attackPos == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(bossContext.attackPos.position, BASE_ATTACK_RANGE);
    }
}