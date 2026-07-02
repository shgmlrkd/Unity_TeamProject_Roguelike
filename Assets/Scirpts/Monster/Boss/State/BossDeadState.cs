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
        bossContext.OnFadeRequested?.Invoke(0.0f, ALPHA_DURATION);
    }

    public override void Tick() {}
}
