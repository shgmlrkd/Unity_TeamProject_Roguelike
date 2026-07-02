using System;
using System.Collections;
using UnityEngine;

public class BossHealState : BossBase
{
    private const float FADE_DURATION = 0.75f;
    private const float PHASE_CHANGE_DELAY = 0.6f;

    private WaitForSeconds fadeWait = new WaitForSeconds(FADE_DURATION);
    private WaitForSeconds phaseChangeWait = new WaitForSeconds(PHASE_CHANGE_DELAY);

    private void OnStartHeal()
    {
        StartCoroutine(HealCoroutine());
    }

    private IEnumerator HealCoroutine()
    {
        // 페이드 아웃
        bossContext.OnFadeRequested?.Invoke(0.0f, FADE_DURATION);
        yield return fadeWait;

        // 힐 및 외형 변경
        bossContext.OnHealRequested?.Invoke();
        bossContext.OnSpriteLibraryChanged?.Invoke(bossContext.data.AngrySpriteLibrary);

        // 변신 딜레이
        yield return phaseChangeWait;

        // 페이드 인
        bossContext.OnFadeRequested?.Invoke(1.0f, FADE_DURATION);
        yield return fadeWait;

        // 무적 끝 (변신 완료)
        bossContext.OnInvincibleChanged?.Invoke(false);

        ChangeState(BossStateEnum.Idle);
    }

    public override void Enter()
    {
        // 무적 시작 (변신 중)
        bossContext.OnInvincibleChanged?.Invoke(true);

        bossContext.animController.OnBossHealTrigger();
    }

    public override void Tick(){ }
}