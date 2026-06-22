using UnityEngine;
using System.Collections.Generic;

public class RoomManager1 : MonoBehaviour
{
    public static RoomManager1 Instance { get; private set; }

    [Header("모듈 연결")]
    [SerializeField] private RoomGenerator1 generator;
    [SerializeField] private RoomTeleporter teleporter;

    [Header("설정")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private DirectionalRoomDatabaseSO roomDatabase;
    [SerializeField] private float roomWidth = 18f, roomHeight = 9f, tileOffset = 3.0f;

    private Dictionary<Vector2Int, RoomInfo> dungeonMap = new Dictionary<Vector2Int, RoomInfo>();
    private Dictionary<RoomType, Dictionary<Vector2Int, List<RoomInfo>>> dbLookUp = new Dictionary<RoomType, Dictionary<Vector2Int, List<RoomInfo>>>();

    private void Awake()
    {
        Instance = this;
        InitializeDatabaseLookUp();
    }

    private void Start()
    {
        generator.CreateRoom(roomDatabase.startRoom, playerTransform.position, Vector2Int.zero);
        dungeonMap[Vector2Int.zero] = roomDatabase.startRoom;
    }

    // 문(Door)을 통과할 때 호출되어 새로운 방을 생성하고 플레이어를 이동시키는 함수
    public void BuildRoomFromDoor(Doorinstall doorSensor, Transform player)
    {
        Vector2Int dir = doorSensor.WallDirection;
        // 이전 방식: 다음 격자 좌표 계산
        Vector2Int nextGridPos = doorSensor.ParentRoomGridPos + dir;

        if (dungeonMap.ContainsKey(nextGridPos)) return;

        float dist = (dir.x != 0) ? roomWidth : roomHeight;
        Vector3 nextRoomPos = doorSensor.transform.position + ((Vector3)(Vector2)dir * dist);

        RoomInfo info = GetTargetRoomInfo(dir, RoomType.Normal);
        generator.CreateRoom(info, nextRoomPos, nextGridPos);
        dungeonMap[nextGridPos] = info;

        teleporter.TeleportToDoor(player, doorSensor.transform.position, dir, tileOffset);
    }
    // 방 타입과 연결 방향에 따라 사용 가능한 방 리스트 중 하나를 무작위로 반환하는 함수
    private RoomInfo GetTargetRoomInfo(Vector2Int direction, RoomType type)
    {
        if (dbLookUp.TryGetValue(type, out var directionDict))
        {
            if (directionDict.TryGetValue(direction, out List<RoomInfo> roomList) && roomList.Count > 0)
            {
                return roomList[Random.Range(0, roomList.Count)];
            }
        }
        return default;
    }
    // 데이터베이스에 정의된 방들을 방향별로 분류하여 빠르게 검색할 수 있도록 딕셔너리에 저장하는 초기화 함수
    private void InitializeDatabaseLookUp()
    {
        if (roomDatabase == null) return;
        dbLookUp[RoomType.Normal] = new Dictionary<Vector2Int, List<RoomInfo>>();
        dbLookUp[RoomType.Normal][Vector2Int.up] = CombineLists(roomDatabase.normalRooms_Up, roomDatabase.normalRooms_Any);
        dbLookUp[RoomType.Normal][Vector2Int.down] = CombineLists(roomDatabase.normalRooms_Down, roomDatabase.normalRooms_Any);
        dbLookUp[RoomType.Normal][Vector2Int.left] = CombineLists(roomDatabase.normalRooms_Left, roomDatabase.normalRooms_Any);
        dbLookUp[RoomType.Normal][Vector2Int.right] = CombineLists(roomDatabase.normalRooms_Right, roomDatabase.normalRooms_Any);
    }
    // 두 개의 리스트(전용 방, 공용 방)를 병합하여 하나의 리스트로 반환하는 유틸리티 함수
    private List<RoomInfo> CombineLists(List<RoomInfo> specificList, List<RoomInfo> anyList)
    {
        List<RoomInfo> combined = new List<RoomInfo>(specificList ?? new List<RoomInfo>());
        if (anyList != null) combined.AddRange(anyList);
        return combined;
    }
}