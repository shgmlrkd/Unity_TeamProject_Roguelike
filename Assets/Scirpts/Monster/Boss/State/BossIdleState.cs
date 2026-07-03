public class BossIdleState : BossBase
{
    private const float IDLE_TIME = 1.5f;

    private bool isPhaseChanged = false;

    public override void Tick()
    {
        if(!isPhaseChanged && bossContext.IsPhaseTwo)
        {
            isPhaseChanged = true;
            ChangeState(BossStateEnum.AttackSelect);
        }

        if(stateTime > IDLE_TIME)
        {
            ChangeState(BossStateEnum.Chase);
        }
    }
}