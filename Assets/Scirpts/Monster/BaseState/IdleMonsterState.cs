using UnityEngine;

public class IdleMonsterState : MonsterBase
{
    public void AnimEventChangePatrol()
    {
        
        monsterStateManager.SetState(MonsterStateEnum.Patrol);
    }


}
