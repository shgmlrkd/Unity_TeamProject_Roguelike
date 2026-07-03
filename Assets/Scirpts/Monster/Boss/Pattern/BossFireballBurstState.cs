using System.Collections;
using UnityEngine;

public class BossFireballBurstState : BossBase
{
    private const float PHASE_CHANGE_DELAY = 0.3f;

    private WaitForSeconds phaseChangeDelayWait = new(PHASE_CHANGE_DELAY);

    private bool useOffsetThisTime = false;
    private bool hasTriggeredPhaseTwo = false; 

    private void OnStartFireballBurst()
    {
        print("투사체 첫번째 발사!");
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

        SoundManager.Instance.PlaySFX(SoundKey.BossAxAttack);
        SoundManager.Instance.PlaySFX(SoundKey.BossFireBall);

        StartCoroutine(FireballBurstEndCoroutine());
    }

    private IEnumerator FireballBurstEndCoroutine()
    {
        yield return phaseChangeDelayWait;

        if (!hasTriggeredPhaseTwo && bossContext.IsPhaseTwo)
        {
            print("투사체 한번 더 함 대기 상태로 돌아가자");
            hasTriggeredPhaseTwo = true;
            bossContext.animController.OnBossPhaseTwo(true);
            yield break;
        }

        print("투사체 끝 대기 상태로 돌아가자");
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