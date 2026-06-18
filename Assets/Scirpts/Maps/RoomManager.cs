using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Start, Normal}
public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public struct RoomData
    {
        public Vector2Int gridPos;
        public RoomType type;
    }

    [Header("맵 생성 설정")]
    [SerializeField] private int maxRooms = 6;      // 생성할 총 방의 개수
    [SerializeField] private float roomWidth;   // 맵 제작자가 정한 방 가로 규격
    [SerializeField] private float roomHeight;  // 맵 제작자가 정한 방 세로 규격

    [Header("방 프리팹")]
    [SerializeField] private GameObject startRoomPrefab; // 시작 방 
    [SerializeField] private List<GameObject> normalRoomPrefabs; // 다양한 디자인의 일반방 리스트

    // 가상 지도 데이터를 담을 딕셔너리 (좌표, 방 정보)
    private Dictionary<Vector2Int, RoomData> dungeonMap = new Dictionary<Vector2Int, RoomData>();
    // 생성된 순서대로 좌표를 담아둘 리스트
    private List<Vector2Int> roomPositions = new List<Vector2Int>();

    private void Start()
    {
        GenerateRoom();
    }

    private void GenerateRoom()
    {
        dungeonMap.Clear();
        roomPositions.Clear();

        // 1단계: (0,0)에 시작 방 배치
        Vector2Int currentPos = Vector2Int.zero;
        RoomData startRoom = new RoomData { gridPos = currentPos, type = RoomType.Start };
        dungeonMap.Add(currentPos, startRoom);
        roomPositions.Add(currentPos);

        // 상하좌우 방향 벡터 배열
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // 2단계: Drunkard's Walk 알고리즘으로 일반 방 배치
        while (roomPositions.Count < maxRooms) 
        {
            // 이미 생성된 방들 중 무작위로 하나를 골라 출발점으로 잡음 (가지치기 유도)
            Vector2Int randomStartPos = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2Int nextPos = randomStartPos + directions[Random.Range(0, directions.Length)];

            // 해당 좌표에 아직 방이 없다면 일반 방 추가
            if (!dungeonMap.ContainsKey(nextPos))
            {
                RoomData normalRoom = new RoomData { gridPos = nextPos, type = RoomType.Normal };
                dungeonMap.Add(nextPos, normalRoom);
                roomPositions.Add(nextPos);
            }
        }
        // 3단계: 가상 지도를 바탕으로 유니티 월드에 프리팹 실시간 조립 및 소환
        SpawnRoomPrefabs();
    }

    // 이웃한 방이 딱 1개뿐인 막다른 길을 전부 수집하는 함수
    private List<Vector2Int> FindDeadEnds()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var pos in roomPositions)
        {
            if (pos == Vector2Int.zero) continue; // 시작방은 제외

            int neighborCount = 0;
            foreach (var dir in directions)
            {
                if (dungeonMap.ContainsKey(pos + dir)) neighborCount++;
            }

            if (neighborCount == 1) deadEnds.Add(pos);
        }
        return deadEnds;
    }

    // 특정 원점으로부터 가장 맨해튼 거리가 먼 좌표를 반환하는 함수
    private Vector2Int GetFurthestDeadEnd(Vector2Int origin, List<Vector2Int> deadEnds)
    {
        Vector2Int furthest = deadEnds[0];
        float maxDistance = 0;

        foreach (var pos in deadEnds)
        {
            float distance = Mathf.Abs(pos.x - origin.x) + Mathf.Abs(pos.y - origin.y);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthest = pos;
            }
        }
        return furthest;
    }

    //방소환 및 맵 배치 조립기
    private void SpawnRoomPrefabs()
    {
        foreach (var pair in dungeonMap)
        {
            RoomData room = pair.Value;

            // 격자 좌표를 맵 규격 크기에 맞게 실제 3D 월드 좌표로 변환
            Vector3 worldPos = new Vector3(room.gridPos.x * roomWidth, room.gridPos.y * roomHeight, 0);
            GameObject spawnedRoom = null;

            // 타입에 맞는 프리팹 소환
            switch (room.type)
            {
                case RoomType.Start:
                    spawnedRoom = Instantiate(startRoomPrefab, worldPos, Quaternion.identity);
                    break;
                case RoomType.Normal:
                    // 일반방 리스트중 하나 소환
                    GameObject randomNormalPrefab = normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Count)];
                    spawnedRoom = Instantiate(randomNormalPrefab, worldPos, Quaternion.identity);
                    break;
            }
        }
    }
}
