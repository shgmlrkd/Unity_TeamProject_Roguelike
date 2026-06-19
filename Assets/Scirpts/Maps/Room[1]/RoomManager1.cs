using System.Collections.Generic;
using UnityEngine;

public class RoomManager1 : MonoBehaviour
{
    [Header("던전 구조 데이터베이스")]
    [SerializeField] private DirectionalRoomDatabaseSO roomDatabase;// 각 방향별로 가능한 방 데이터 모음

    [Header("방 크기 규격 설정")]
    [SerializeField] private float roomWidth = 32f; // 방의 가로 크기 
    [SerializeField] private float roomHeight = 21f; // 방의 세로 크기 

    [Header("타일 안전지대 진입 보정값")]
    [SerializeField] private float tileOffset = 2.0f;// 방 이동 시 문 타일에 끼이지 않게 보정하는 거리

    [Header("던전 특수방 생성 규칙")]
    [SerializeField] private int minRoomsBeforeBoss = 5;// 보스방이 등장하기 전 최소 일반방 개수
    [SerializeField] private int storeSpawnInterval = 4;// 상점방이 등장하는 간격

    // 이미 생성된 방들의 좌표를 저장하여 중복 생성을 방지하는 맵 데이터
    private Dictionary<Vector2Int, RoomInfo> dungeonMap = new Dictionary<Vector2Int, RoomInfo>();
    private Dictionary<RoomType, Dictionary<Vector2Int, List<RoomInfo>>> dbLookUp = new Dictionary<RoomType, Dictionary<Vector2Int, List<RoomInfo>>>();
    private RoomRuleChecker1 ruleChecker;

    private void Awake()
    {
        // 규칙 판별기 초기화 및 데이터베이스 검색 테이블 생성
        ruleChecker = new RoomRuleChecker1(minRoomsBeforeBoss, storeSpawnInterval);
        InitializeDatabaseLookUp();
    }

    private void Start()
    {
        SpawnInitialRoom();
    }

    // 문을 통해 다음 방을 생성하고 플레이어를 해당 방의 입구로 순간이동시키는 핵심 로직
    public void BuildRoomAndTeleport(DynamicRoom currentRoom, Vector2Int direction, Transform player)
    {
        //다음 방이 생성될 격자 좌표 계산
        Vector2Int nextGridPos = currentRoom.RoomGridPos + direction;
        Debug.Log($"다음 방 생성 위치: {nextGridPos}");

        //이미 해당 위치에 방이 있는지 확인 (중복 생성 방지)
        if (dungeonMap.ContainsKey(nextGridPos))
        {
            Debug.LogWarning("이미 생성된 위치입니다!"); 
            return;
        }

        //규칙에 따라 다음 방 타입 결정 및 방 정보 가져오기
        RoomType determinedType = ruleChecker.DetermineNextRoomType(direction);
        RoomInfo selectedRoom = GetTargetRoomInfo(direction, determinedType);

        //만약 선택된 타입의 방이 없다면 일반방(Normal)으로 대체
        if (selectedRoom.roomPrefab == null)
        {
            determinedType = RoomType.Normal;
            selectedRoom = GetTargetRoomInfo(direction, RoomType.Normal);
        }
        //생성된 방을 맵 데이터에 등록하고 규칙 상태 업데이트
        dungeonMap.Add(nextGridPos, selectedRoom);
        UpdateRuleState(determinedType);

        //월드 좌표를 계산하여 방 프리팹 인스턴스화
        Vector3 nextRoomWorldPos = new Vector3(nextGridPos.x * roomWidth, nextGridPos.y * roomHeight, 0f);
        GameObject spawnedRoom = Instantiate(selectedRoom.roomPrefab, nextRoomWorldPos, Quaternion.identity);

        // 생성된 방의 컴포넌트에 자신의 격자 위치 정보 전달
        if (spawnedRoom.TryGetComponent<DynamicRoom>(out DynamicRoom roomComp))
        {
            roomComp.SetupRoom(nextGridPos, selectedRoom);
        }

        //플레이어 위치 보정 (이동한 방향의 반대편 입구로 배치) {버그 발생 구역}###
        Vector3 nextRoomCenter = new Vector3(nextGridPos.x * roomWidth, nextGridPos.y * roomHeight, 0f);
        Vector3 spawnLocalPos = Vector3.zero;

        if (direction == Vector2Int.up)
            spawnLocalPos = new Vector3(0f, -(roomHeight / 2f) + tileOffset, 0f);
        else if (direction == Vector2Int.down)
            spawnLocalPos = new Vector3(0f, (roomHeight / 2f) - tileOffset, 0f);
        else if (direction == Vector2Int.left)
            spawnLocalPos = new Vector3((roomWidth / 2f) - tileOffset, 0f, 0f);
        else if (direction == Vector2Int.right)
            spawnLocalPos = new Vector3(-(roomWidth / 2f) + tileOffset, 0f, 0f);

        Vector3 targetWorldPos = nextRoomCenter + spawnLocalPos;
        targetWorldPos.z = player.position.z;

        player.position = targetWorldPos;
    }
    // 보스방 생성 여부나 일반방 카운트를 관리하여 규칙을 갱신
    private void UpdateRuleState(RoomType type)
    {
        if (type == RoomType.Normal) ruleChecker.GeneratedNormalRoomCount++;
        else if (type == RoomType.Boss) ruleChecker.IsBossRoomSpawned = true;
    }
    // 데이터베이스에서 특정 조건에 맞는 방 정보를 랜덤하게 가져옴
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
    // 게임 시작 시 던전의 시작점이 되는 (0,0) 방을 생성 {이것도 좌표 조정 필요!!!}
    private void SpawnInitialRoom()
    {
        if (roomDatabase == null || roomDatabase.startRoom.roomPrefab == null) return;

        Vector2Int startPos = Vector2Int.zero;
        dungeonMap.Add(startPos, roomDatabase.startRoom);

        GameObject spawnedRoom = Instantiate(roomDatabase.startRoom.roomPrefab, Vector3.zero, Quaternion.identity);
        if (spawnedRoom.TryGetComponent<DynamicRoom>(out DynamicRoom roomComp))
        {
            roomComp.SetupRoom(startPos, roomDatabase.startRoom);
        }
    }
    // DB에 있는 데이터를 타입별, 방향별로 분류하여 빠르게 조회할 수 있게 딕셔너리 구성
    private void InitializeDatabaseLookUp()
    {
        if (roomDatabase == null) return;

        dbLookUp[RoomType.Normal] = new Dictionary<Vector2Int, List<RoomInfo>>();
        dbLookUp[RoomType.Boss] = new Dictionary<Vector2Int, List<RoomInfo>>();
        dbLookUp[RoomType.Store] = new Dictionary<Vector2Int, List<RoomInfo>>();
        dbLookUp[RoomType.Treasure] = new Dictionary<Vector2Int, List<RoomInfo>>();
        // 방향별로 리스트 결합 (특정 방향 전용 방 + 어느 방향이든 가능한 방)
        dbLookUp[RoomType.Normal][Vector2Int.up] = CombineLists(roomDatabase.normalRooms_Up, roomDatabase.normalRooms_Any);
        dbLookUp[RoomType.Normal][Vector2Int.down] = CombineLists(roomDatabase.normalRooms_Down, roomDatabase.normalRooms_Any);
        dbLookUp[RoomType.Normal][Vector2Int.left] = CombineLists(roomDatabase.normalRooms_Left, roomDatabase.normalRooms_Any);
        dbLookUp[RoomType.Normal][Vector2Int.right] = CombineLists(roomDatabase.normalRooms_Right, roomDatabase.normalRooms_Any);

        dbLookUp[RoomType.Boss][Vector2Int.up] = roomDatabase.bossRooms_Up;
        dbLookUp[RoomType.Boss][Vector2Int.down] = roomDatabase.bossRooms_Down;
        dbLookUp[RoomType.Boss][Vector2Int.left] = roomDatabase.bossRooms_Left;
        dbLookUp[RoomType.Boss][Vector2Int.right] = roomDatabase.bossRooms_Right;

        dbLookUp[RoomType.Store][Vector2Int.up] = roomDatabase.storeRooms_Up;
        dbLookUp[RoomType.Store][Vector2Int.down] = roomDatabase.storeRooms_Down;
        dbLookUp[RoomType.Store][Vector2Int.left] = roomDatabase.storeRooms_Left;
        dbLookUp[RoomType.Store][Vector2Int.right] = roomDatabase.storeRooms_Right;

        dbLookUp[RoomType.Treasure][Vector2Int.up] = roomDatabase.treasureRooms_Up;
        dbLookUp[RoomType.Treasure][Vector2Int.down] = roomDatabase.treasureRooms_Down;
        dbLookUp[RoomType.Treasure][Vector2Int.left] = roomDatabase.treasureRooms_Left;
        dbLookUp[RoomType.Treasure][Vector2Int.right] = roomDatabase.treasureRooms_Right;
    }
    // 두 개의 방 리스트를 하나로 합쳐서 반환
    private List<RoomInfo> CombineLists(List<RoomInfo> specificList, List<RoomInfo> anyList)
    {
        List<RoomInfo> combined = new List<RoomInfo>(specificList);
        if (anyList != null) combined.AddRange(anyList);
        return combined;
    }
}