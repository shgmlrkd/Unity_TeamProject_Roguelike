using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarGrid : MonoBehaviour
{
    private AStarNode[,] nodes; // 2차원 배열의 노드 배열 선언
    private Tilemap wallTilemap; // 좌표 변환 / 맵 크기 용
    private LayerMask obstacleLayer; // 벽 판정용

    BoundsInt bounds; // 타일맵의 범위와 크기 정보 저장



    private void Awake()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");

        wallTilemap = GetComponentInChildren<Tilemap>();
        CreateGrid();
    }

    private void CreateGrid() // 타일맵 측정후 배열에 노드 넣기
    {
        bounds = wallTilemap.cellBounds; // 타일 맵 측정

        int sizeX = bounds.size.x;
        int sizeY = bounds.size.y;

        nodes = new AStarNode[sizeX, sizeY];  // 측정된 타일맵 만큼 배열 만들기

        for (int x = 0; x < sizeX; x++) // 모든 배열 칸에 노드 생성
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector2Int nodePos = new Vector2Int(bounds.xMin + x, bounds.yMin + y); // 배열 인덱스와 실제 타일 좌표 맞추기

                Vector3Int tilePos = new Vector3Int(nodePos.x, nodePos.y, 0); // 타일맵에서 해당 좌표 검색하기 위한 좌표 생성

                Vector3 worldPos = wallTilemap.GetCellCenterWorld(tilePos); // 타일 좌표를 월드 중앙 좌표로 변환

                Collider2D wall = Physics2D.OverlapBox(worldPos, wallTilemap.cellSize * 0.8f, 0f, obstacleLayer); // obstacleLayer 체크

                bool hasWall = wall != null;

                nodes[x, y] = new AStarNode(nodePos, hasWall); // 노드 생성후 배열에 저장
            }
        }
    }

    private AStarNode GetNode(Vector2Int cellPos) // 타일 좌표에 해당하는 노드 반환
    {

        int x = cellPos.x - bounds.xMin;
        int y = cellPos.y - bounds.yMin;

        if (x < 0 || x >= nodes.GetLength(0) || y < 0 || y >= nodes.GetLength(1)) // 배열 범위 밖이라면 null
        {
            return null;
        }
        return nodes[x, y];
    }

    public AStarNode GetNodeFromWorld(Vector3 worldPos) // 필드 트랜스폼의 포지션을 타일 좌표로 변환
    {
        Vector3Int cellPos = wallTilemap.WorldToCell(worldPos);
        Vector2Int nodePos = new Vector2Int(cellPos.x, cellPos.y);

        return GetNode(nodePos); // A_StarNode 반환
    }

    public List<AStarNode> GetNeighbors(AStarNode currentNode)  // 갈수 있는 길의 후보를 리스트에 넣기
    {
        List<AStarNode> neighbors = new List<AStarNode>();

        Vector2Int[] dirs = {
        new Vector2Int(1, 0),   // 오른쪽 
        new Vector2Int(-1, 0),  // 왼쪽
        new Vector2Int(0, 1),   // 위쪽
        new Vector2Int(0, -1),  // 아래쪽

        new Vector2Int(1, 1),   // 오른쪽 위
        new Vector2Int(-1, 1),  // 왼쪽 위
        new Vector2Int(-1, -1), // 왼쪽 아래
        new Vector2Int(1, -1), // 오른쪽 아래

        };

        foreach (Vector2Int dir in dirs)  // 8 방향 탐색
        {
            AStarNode node = GetNode(currentNode.Position + dir);  // 현재 검사한 주변 노드 가져오기

            if (node == null) continue; // 맵밖 이면 무시
            if (node.IsWall) continue;  // 벽이면 무시

            if (dir.x != 0 && dir.y != 0) // 대각선 이동을 하는데 벽사이로 가는것을 방지
            {
                AStarNode sideX = GetNode(currentNode.Position + new Vector2Int(dir.x, 0)); // 대각선 검사 
                AStarNode sideY = GetNode(currentNode.Position + new Vector2Int(0, dir.y)); // 대각선 검사 

                if (sideX == null || sideY == null) continue;
                if (sideX.IsWall || sideY.IsWall) continue;

            }


            neighbors.Add(node); // 갈수 있는 노드 저장
        }

        return neighbors; // 주변의 이동 가능한 노드 반환

    }

    public Vector3 GetWorldPosition(AStarNode node) // 타일 좌표 변환 메서드 
    {
        Vector3Int cellPos = new Vector3Int(node.Position.x, node.Position.y, 0);  // 노드가 가진 타일 좌표를 Tilemap 셀 좌표로 변환

        return wallTilemap.GetCellCenterWorld(cellPos); // 해당 셀의 월드중앙 좌표 반환
    }


    public void ResetNode()  // 초기화 용 메서드
    {
        foreach(AStarNode node in nodes)
        {
            node.ResetCost();
        }
    }

    public AStarNode GetRandomPatrolNode(AStarNode startNode, float minDistance, float maxDistance) // 패트롤 랜덤 end 포지션 지정 메서드
    {
        List<AStarNode> randomPatrolNodes = new List<AStarNode>(); // 이동 가능한 노드만 저장할 리스트
        
        foreach (AStarNode node in nodes)
        {
            if (node == null) continue;
            if (node.IsWall) continue;

            float distance = Vector2.Distance(startNode.Position, node.Position);

            if (distance < minDistance) continue;
            if (distance > maxDistance) continue;

            randomPatrolNodes.Add(node);// 이동 가능한 노드 리스트에 추가

        }

        if (randomPatrolNodes.Count == 0)// 이동 가능한 노드가 없다면 리턴 null
        {
            return null;
        }

        int randomIndex = Random.Range(0, randomPatrolNodes.Count);// 이동 가능한 노드 리스트 범위 안에서 랜덤 인덱스 선택


        return randomPatrolNodes[randomIndex]; // 랜덤하게 선택된 이동 가능한 노드 반환

    }

}

