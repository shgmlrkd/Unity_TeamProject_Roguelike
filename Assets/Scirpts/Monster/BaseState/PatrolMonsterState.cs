using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PatrolMonsterState : MonsterBase
{
    // 대기시간 및 코루틴 저장용
    private WaitForSeconds wait;
    private Coroutine patrolCo;

    private int pathIndex;                              // 목표로 하는 노드 번호

    protected override void Awake() 
    {
        base.Awake();
        wait = new WaitForSeconds(monsterStateManager.MonsterData.PatrolWaitTime); // 정찰 대기 시간
    }
 
    private void OnEnable() // 순찰 시작
    {
        pathFinder = monsterStateManager.PathFinder;
        patrolCo = StartCoroutine(PatrolCo());

    }

    private void OnDisable() // 코루틴 종료
    {
        currentPath = null; // 경로 정보 초기화
        pathIndex = 0;

        if (patrolCo != null)
        {
            StopCoroutine(patrolCo);
            patrolCo = null;
        }
        
    }

    private IEnumerator PatrolCo()  // 랜덤 위치 순찰
    {
        bool hasPath = TrySetRandomPath(); // 랜덤 순찰 목표까지의 경로 생성
        print($"경로 생성완료 : {hasPath}");
        if (hasPath) // 목표 경로가 있다면 이동
        {
            yield return MovePath();
            print("패트롤 끝");
        }
        else
        {
            print("경로 못받음");
        }
        monsterStateManager.SetState(MonsterStateEnum.Idle);

        yield break; // 도착후 대기

    }

    private IEnumerator MovePath()
    {
        while ( currentPath != null && pathIndex < currentPath.Count) // 경로가 존재하며 목적지에 도착하지 않았다면
        {
            print("현재 목표 노드의 월드 좌표 가져오기 시작");
            Vector3 targetPos = pathFinder.Grid.GetWorldPosition(currentPath[pathIndex]); // 현재 목표 노드의 월드 좌표 가져오기
            print($"현재 목표 노드의 월드 좌표 옴 {targetPos}");
            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                targetPos,
                monsterStateManager.MonsterData.PatrolSpeed * Time.fixedDeltaTime); // 목표까지 이동
            print($"다음 경로 {nextPosition}");
            rb.MovePosition(nextPosition);

            monsterStateManager.FlipTo(targetPos);

            if (Vector2.Distance(nextPosition, targetPos) < 0.15f) // 목표 노드에 도착했다면 다음 목표 노드로 변경 도착 판정을 널널하게
            {
                print($"도착 {pathIndex}");
                pathIndex++;
                print($"인덱스 변경 {pathIndex}");
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private bool TrySetRandomPath()
    {
        // 현재 몬스터 위치에 해당하는 시작 노드 가져오기
        AStarNode startNode = pathFinder.Grid.GetNodeFromWorld(rb.position);
       
        // 못 가져 왔다면 실패
        if(startNode == null)
        {
            return false;
        }

        // 시작 노드 기준 최소 / 최대 거리 안에서 랜덤 순찰 노드
        AStarNode randomNode = pathFinder.Grid.GetRandomPatrolNode(
            startNode,
            monsterStateManager.MonsterData.MinPatrolDistance,
            monsterStateManager.MonsterData.MaxPatrolDistance);

        if(randomNode == null)
        {
            print("랜덤노느 못가져옴");
            return false;
        }
        
        // 랜덤으로 뽑은 노드를 월드 좌표로 전환
        Vector3 targetPos = pathFinder.Grid.GetWorldPosition(randomNode);
        print($"현재 경로 변수 : {currentPath}");
        print($"목표 좌표 : {targetPos}");
        // 현재 위치에서 랜덤 목표 위치까지 경로 생성
        currentPath = pathFinder.FindPath(rb.position, targetPos);

        // 경로가 없거나
        // 현재 위치만 들어있는 경로면 실패
        if (currentPath == null || currentPath.Count <= 1)
        {
            print("경로가 없거나 현재위치만 있음");
            return false;
        }

        // currentPath[0] 은 현재 위치
        // 실제 이동은 다음 노드부터 시작
        pathIndex = 1;

        // 랜덤 순찰 경로 생성 성공
        return true;

    }

}
