using UnityEngine;

public class BossDeadState : BossBase
{
    private const float ALPHA_DURATION = 0.95f;

    public override void Enter()
    {
        bossContext.animController.OnBossDeadTringger();
    }
    private void OnStartFadeOut()
    {
        bossContext.RequestFade(0.0f, ALPHA_DURATION);
    }

    private void OnPlayDeadSFX()
    {
        SoundManager.Instance.PlaySFX(SoundKey.BossDead);
    }

    private void OnPlayDeadIntroSFX()
    { 
        SoundManager.Instance.PlaySFX(SoundKey.BossDeadIntro); 
    }

    public override void Tick() {}
}
