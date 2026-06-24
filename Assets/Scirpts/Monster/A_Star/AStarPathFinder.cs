using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    private int straightCost = 10; // 이동 비용
    private int diagonalCost = 14;
    private const int minNode = 4;
    private const int maxNode = 8;

    private AStarGrid grid;
    private List<AStarNode> openList;
    private List<AStarNode> closeList;

    public AStarGrid Grid { get { return grid; } }

    private void Awake()
    {
        grid = GetComponent<AStarGrid>();
    }

    private int GetDistance(AStarNode NodeA, AStarNode NodeB)
    {
        int dstX = Mathf.Abs(NodeA.Position.x - NodeB.Position.x);  // x 축 차이 개수
        int dstY = Mathf.Abs(NodeA.Position.y - NodeB.Position.y);  // y 축 차이 개수

        if(dstX > dstY)
        {
            return diagonalCost * dstY + straightCost * (dstX - dstY); // 대각선 이동 비용 + 남은 직선 이동 비용
        }
        return diagonalCost * dstX + straightCost * (dstY - dstX); // 대각선 이동 비용 + 남은 직선 이동 비용

    }

    private List<AStarNode> RetracePath(AStarNode startNode, AStarNode targetNode)
    {
        List<AStarNode> path = new List<AStarNode>(); // 최종 경로를 저장할 리스트

        AStarNode currentNode = targetNode;            // 경로 복원을 위해 목표 노드 부터 시작

        while (currentNode != startNode)                // 시작 노드에 도착할 때 까지
        {
            path.Add(currentNode);                      // 현재 노드를 경로 리스트에 저장
            currentNode = currentNode.Parent;           // 부모 노드로 이동
        }

        path.Add(startNode);        // 반복이 끝나고 경로에 시작점 추가

        path.Reverse();             // 저장된 순서 뒤집어서 스타트가 먼저 인식되게함

        int randomLength = Random.Range(minNode, maxNode); // 3~7 민 맥스 상수 변수

        if (path.Count > randomLength)
        {
            path = path.GetRange(0, randomLength);
        }

        return path;
    }
    public List<AStarNode> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        AStarNode startNode = grid.GetNodeFromWorld(startPos);     // 시작 위치 가져오기
        AStarNode targetNode = grid.GetNodeFromWorld(targetPos);   // 목표 위치 노드 가져오기
        

        if(startNode == null || targetNode == null)
        {
            return null;
        }
        if(startNode.IsWall ||  targetNode.IsWall)
        {
            return null;
        }

        openList = new List<AStarNode>();      // 초기화
        closeList = new List<AStarNode>();     // 초기화

        grid.ResetNode();               // 이전에 탐색에서 사용된 G, H, Parent 값 초기화

        startNode.G = 0;                // 시작점에서 시작점까지 비용은 0 (초기화: MaxValue에서 0으로)
        startNode.H = GetDistance(startNode, targetNode);  // 시작점에서 목표 지점까지의 예상 비용 계산 (초기화)

        openList.Add(startNode);        // 시작노드 openList에 넣기

        while(openList.Count > 0)
        {
            AStarNode currentNode = openList[0];       // 임시 최고 순위 후보 지정

            for (int i = 1; i < openList.Count; i++)    // 최고 순위를 제외한 더 좋은 노드 비교
            {
                if(openList[i].F < currentNode.F || openList[i].F == currentNode.F && openList[i].H < currentNode.H) 
                    // open 리스트 중 비용 비교를 해서 더 좋은 후보 찾기
                {
                    currentNode = openList[i]; // 더 좋은 후보를 현재 노드로 변경
                }
            }

            openList.Remove(currentNode);       // 이번에 검사할 노드는 후보에서 제거
            closeList.Add(currentNode);         // 검사 완료된 목록에 추가

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (AStarNode neighbor in grid.GetNeighbors(currentNode))  
            {
                if (neighbor.IsWall || closeList.Contains(neighbor))        // 주변 노드가 벽 또는 검사 끝난 노드는 할필요 없으니 다시 다음으로 진행
                {
                    continue;
                }

                int newCost = currentNode.G + GetDistance(currentNode, neighbor);   // 총 이동 비용 newCost 에 저장

                if(newCost < neighbor.G || !openList.Contains(neighbor))            // 기존보다 더 좋은 길을 찾았거나 또는 처음 발견한 노드라면
                {
                    neighbor.G = newCost;                   // 시작점에서 부터 neighbor 까지의 새 G의 비용

                    neighbor.H = GetDistance(neighbor, targetNode);    // neighbor에서 목표까지의 예상 비용

                    neighbor.Parent = currentNode;          // 이 노드에 오기 직전의 노드를 저장

                    if(!openList.Contains(neighbor)) // 검사 하는 목록에 없는 노드라면 
                    {
                        openList.Add(neighbor);      // 검사 할 목록 리스트에 넣는다.
                    }

                }

            }

        }
        return null;
    }

}
