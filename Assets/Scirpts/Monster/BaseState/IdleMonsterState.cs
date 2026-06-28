
public class IdleMonsterState : MonsterBase
{
    public void AnimEventChangePatrol()
    {
        if (!monsterStateManager.IsStartCheckState)
        {
            print(monsterStateManager.IsStartCheckState);
            return;
        }
        monsterStateManager.SetState(MonsterStateEnum.Patrol);
    }


}
