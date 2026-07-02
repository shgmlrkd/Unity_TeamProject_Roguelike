using System;
using UnityEngine;

public class BossDashAttackState : BossBase
{
    private const float DASH_START = 2.5f;
    private const float DASH_RATE = 3.5f;
    private const float HIT_CHECK_RADIUS = 0.5f;

    private Vector2 dashDirection;

    private bool isDashing = false;
    private bool isStunned = false; // 벽 충돌 후 스턴 대기 상태
    private bool hasRetriedDash = false;
    private void Start()
    {
        bossContext.OnPhaseTwoRequested += OnPhaseTwo;
    }

    private void OnDisable()
    {
        bossContext.OnPhaseTwoRequested -= OnPhaseTwo;
    }

    public override void Enter()
    {
        base.Enter();

        isDashing = false;
        isStunned = false;
        hasRetriedDash = false;

        bossContext.animController.OnBossStunned(false);
        bossContext.animController.OnBossDashAttackTrigger();
    }

    public override void Tick()
    {
        // 스턴 중에는 못하게 하기
        if (isStunned) return; 

        if (!isDashing && CanDash())
        {
            isDashing = true;
            dashDirection = GetDirection();
            bossContext.animController.OnBossDashStartTrigger();
        }
    }

    public override void FixedTick()
    {
        if (!isDashing)
            return;

        bossContext.rb.MovePosition(
            bossContext.rb.position +
            dashDirection * bossContext.CurrentMoveSpeed * DASH_RATE * Time.fixedDeltaTime);

        CheckPlayerHit();
    }

    private void CheckPlayerHit()
    {
        LayerMask playerLayerMask = 1 << bossContext.target.gameObject.layer;
        
        Collider2D hit = Physics2D.OverlapCircle(bossContext.rb.position, HIT_CHECK_RADIUS, playerLayerMask);
        
        if (hit == null) return;

        if (hit.TryGetComponent(out IDamageable player))
        {
            bossContext.animController.OnBossDashSuccess();
            DamageInfoSet damage = new DamageInfoSet(bossContext.CurrentAttackDamage);
            player.TakeDamage(damage);
            ChangeState(BossStateEnum.Idle);
        }
    }
    private void OnPhaseTwo()
    {
        if (!isStunned)
            return;

        OnBossStunEnd();
    }

    private bool CanDash()
    {
        return stateTime >= DASH_START;
    }

    public void OnBossStunEnd()
    {
        bossContext.animController.OnBossStunned(true);
        ChangeState(BossStateEnum.Idle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing)
            return;

        // 장애물이 아니면 반환
        if (collision.gameObject.layer != LayerMask.NameToLayer("Obstacle")) return;

        isDashing = false;

        // 페이즈2에서는 한 번 더 돌진
        if (bossContext.IsPhaseTwo && !hasRetriedDash)
        {
            hasRetriedDash = true;

            dashDirection = GetDirection();
            bossContext.animController.OnBossDashStartTrigger();
            isDashing = true;
            return;
        }

        bossContext.animController.OnBossDashFail();
        isDashing = false;
        isStunned = true;
        stateTime = 0.0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HIT_CHECK_RADIUS);
    }
}