using System.Collections;
using UnityEngine;

public class BossFireballBurstState : BossBase
{
    private const float PHASE_CHANGE_DELAY = 0.25f;

    private WaitForSeconds phaseChangeDelayWait = new(PHASE_CHANGE_DELAY);

    private bool useOffsetThisTime = false;
    private bool hasTriggeredPhaseTwo = false; 

    private void OnStartFireballBurst()
    {
        float angleOffset = FULL_CIRCLE_ANGLE / bossContext.FireballCount * 0.5f;

        for (int i = 0; i < bossContext.FireballCount; i++)
        {
            MonsterBullet bossBullet = MonsterManager.Instance.GetBossBullet();

            float startAngle = useOffsetThisTime ? angleOffset : 0.0f;

            Vector3 direction = GetRadialDirection(i, bossContext.FireballCount, startAngle);

            Vector2 firePos = bossContext.firePos.position;

            bossBullet.Init(
                direction,
                firePos,
                bossContext.data.ProjectileSpeed,
                bossContext.CurrentAttackDamage,
                bossBullet.gameObject);
        }

        useOffsetThisTime = !useOffsetThisTime;

        StartCoroutine(FireballBurstEndCoroutine());
    }

    private IEnumerator FireballBurstEndCoroutine()
    {
        yield return phaseChangeDelayWait;

        if (!hasTriggeredPhaseTwo && bossContext.IsPhaseTwo)
        {
            hasTriggeredPhaseTwo = true;
            bossContext.animController.OnBossPhaseTwo(true);
            yield break;
        }

        ChangeState(BossStateEnum.Idle);
    }

    public override void Enter()
    {
        useOffsetThisTime = false;
        hasTriggeredPhaseTwo = false;

        bossContext.animController.OnBossFireballBurstTrigger();
    }

    public override void Tick(){}
}