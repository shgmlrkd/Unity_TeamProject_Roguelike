using UnityEngine;
using UnityEngine.Splines;

public class ChaseMonsterState : MonsterBase
{
    private Transform player;
    private Vector2 nextPosition;

    private void FixedUpdate()
    {
        player = monsterStateManager.Target; // 상태매니저에서 가져온 타겟을 트랜스폼에 저장후 사용

        if (player == null) return;

        // 쫒아가기
        nextPosition = Vector3.MoveTowards(
           rb.position,
           player.position,
           monsterStateManager.MonsterData.MoveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(nextPosition);
    }
   
}
