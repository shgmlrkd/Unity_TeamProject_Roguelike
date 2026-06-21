using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ChaseMonsterState : MonsterBase
{
    
    private A_StarPathFinder pathFinder;
    private int pathIndex;
    private float pathTimer;

    [SerializeField] private float pathUpdateTime = 0.3f;
    
    private Transform player;
    
    List<A_StarNode> currentPath;
    

    protected override void Awake()
    {
        base.Awake();
        pathFinder = monsterStateManager.PathFinder;
    }

    private void FixedUpdate()
    {
        player = monsterStateManager.Target; // 상태매니저에서 가져온 타겟을 트랜스폼에 저장후 사용

        if (player == null) return;
        
        pathTimer += Time.fixedDeltaTime;

        if (pathTimer >= pathUpdateTime)
        {
            pathTimer = 0.0f;

            currentPath = pathFinder.FindPath(rb.position, player.position);
            pathIndex = 1;
        }


        MovePath();
    }
    private void MovePath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (pathIndex >= currentPath.Count) return;

        Vector3 targetPos = pathFinder.Grid.GetWorldPosition(currentPath[pathIndex]);

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            targetPos,
            monsterStateManager.MonsterData.MoveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(nextPosition);

        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            pathIndex++;
        }
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null) return;

        Gizmos.color = Color.purple;

        foreach (A_StarNode node in currentPath)
        {
            Vector3 worldPos = pathFinder.Grid.GetWorldPosition(node);

            Gizmos.DrawSphere(worldPos, 0.3f);

            
        }

        
        
    }

}
