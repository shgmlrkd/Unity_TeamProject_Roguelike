using Cainos.PixelArtTopDown_Basic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager1 : ScenesSingleton<RoomManager1>
{
    private bool isChangingRoom = false; // 이동 중임을 나타내는 플래그
    [Header("모듈 연결")]
    [SerializeField] private RoomGenerator1 generator;
    [SerializeField] private RoomTeleporter teleporter;

    [Header("설정")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private DirectionalRoomDatabaseSO roomDatabase;
    [SerializeField] private float roomWidth = 18f, roomHeight = 9f, tileOffset = 3.0f;

    private Dictionary<Vector2Int, RoomInfo> dungeonMap = new Dictionary<Vector2Int, RoomInfo>();
    private Dictionary<RoomType, Dictionary<Vector2Int, List<RoomInfo>>> dbLookUp = new Dictionary<RoomType, Dictionary<Vector2Int, List<RoomInfo>>>();
    private Dictionary<Vector2Int, Vector3> roomPositions = new Dictionary<Vector2Int, Vector3>();

    protected override void Awake()
    {
        base.Awake();
        InitializeDatabaseLookUp();
    }

    private void Start()
    {
        generator.CreateRoom(roomDatabase.startRoom, playerTransform.position, Vector2Int.zero);
        dungeonMap[Vector2Int.zero] = roomDatabase.startRoom;
        roomPositions[Vector2Int.zero] = playerTransform.position;
    }
    private void LateUpdate()
    {
        if (isChangingRoom) return;

        Vector2Int currentGrid = GetCurrentRoomGridPos(playerTransform.position);

        if (roomPositions.TryGetValue(currentGrid, out Vector3 targetPos))
        {
            Vector3 targetCamPos = new Vector3(targetPos.x, targetPos.y, -10);

            // 카메라가 현재 위치와 아주 미세하게 다르다면(0.1f 이상 차이 시) 이동
            if (Vector3.Distance(Camera.main.transform.position, targetCamPos) > 0.1f)
            {
                Camera.main.transform.position = targetCamPos;
            }
        }
    }

    // 플레이어 위치를 격자 좌표로 변환하는 함수
    private Vector2Int GetCurrentRoomGridPos(Vector3 playerPos)
    {
        Vector2Int bestGrid = Vector2Int.zero;
        float minDistance = float.MaxValue;

        // 저장된 모든 방 위치 중 플레이어와 가장 가까운 방을 찾음
        foreach (var kvp in roomPositions)
        {
            float dist = Vector3.Distance(playerPos, kvp.Value);
            if (dist < minDistance)
            {
                minDistance = dist;
                bestGrid = kvp.Key;
            }
        }
        return bestGrid;
    }
    // 문(Door)을 통과할 때 호출되어 새로운 방을 생성하고 플레이어를 이동시키는 함수
    public void BuildRoomFromDoor(Doorinstall doorSensor, Transform player)
    {
        var camFollow = Camera.main.GetComponent<CameraFollow>();
        if (camFollow != null) camFollow.enabled = false;
        isChangingRoom = true;
        Vector2Int dir = doorSensor.WallDirection;
        Vector2Int nextGridPos = doorSensor.ParentRoomGridPos + dir;

        // 1. 이미 방이 있는지 확인
        if (dungeonMap.ContainsKey(nextGridPos))
        {
            // 방이 있다면 좌표를 가져옴 (TryGetValue를 써야 에러가 안 납니다)
            if (roomPositions.TryGetValue(nextGridPos, out Vector3 targetPos))
            {
                Camera.main.transform.position = new Vector3(targetPos.x, targetPos.y, -10);
            }
            teleporter.TeleportToDoor(player, doorSensor.transform.position, dir, tileOffset);
        }
        else
        {
            // 2. 방이 없다면: 새로 생성
            float dist = (dir.x != 0) ? roomWidth : roomHeight;
            Vector3 nextRoomPos = doorSensor.transform.position + ((Vector3)(Vector2)dir * dist);

            RoomInfo info = GetTargetRoomInfo(dir, RoomType.Normal);
            generator.CreateRoom(info, nextRoomPos, nextGridPos);

            // 중요: 생성된 방을 맵 데이터에 기록
            dungeonMap[nextGridPos] = info;
            roomPositions[nextGridPos] = nextRoomPos;
            Camera.main.transform.position = new Vector3(nextRoomPos.x, nextRoomPos.y, -10);

            teleporter.TeleportToDoor(player, doorSensor.transform.position, dir, tileOffset);
        }
        StartCoroutine(ResetCameraLock(0.2f));
    }
    private IEnumerator ResetCameraLock(float delay)
    {
        // 0.2초 대기
        yield return new WaitForSeconds(delay);

        // 1. 대기가 끝난 직후, 플레이어의 현재 위치를 다시 한번 확실히 파악
        Vector2Int currentGrid = GetCurrentRoomGridPos(playerTransform.position);

        // 2. 만약 카메라가 현재 위치와 다른 곳을 보고 있다면 마지막으로 한번 더 강제 세팅
        if (roomPositions.ContainsKey(currentGrid))
        {
            Vector3 targetPos = roomPositions[currentGrid];
            Camera.main.transform.position = new Vector3(targetPos.x, targetPos.y, -10);
        }

        // 3. 이제 LateUpdate에게 제어권 반환
        isChangingRoom = false;
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