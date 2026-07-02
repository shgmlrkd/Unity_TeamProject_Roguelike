public class BossIdleState : BossBase
{
    private const float IDLE_TIME = 1.5f;

    public override void Tick()
    {
        if(stateTime > IDLE_TIME)
        {
            ChangeState(BossStateEnum.Chase);
        }
    }
}