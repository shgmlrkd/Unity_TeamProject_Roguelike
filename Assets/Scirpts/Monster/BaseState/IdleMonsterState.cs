using UnityEngine;

public class IdleMonsterState : MonsterBase
{
    public void AnimEventChangePatrol()
    {
        if (!monsterStateManager.IsStartCheckState) return;
        monsterStateManager.SetState(MonsterStateEnum.Patrol);
    }


}
