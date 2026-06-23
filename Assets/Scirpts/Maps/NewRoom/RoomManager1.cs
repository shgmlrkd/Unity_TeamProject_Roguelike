using Cainos.PixelArtTopDown_Basic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager1 : ScenesSingleton<RoomManager1>
{
    private bool isChangingRoom = false; // 이동 중임을 나타내는 플래그
    private RoomRuleChecker1 ruleChecker = new RoomRuleChecker1();
    [Header("모듈 연결")]
    [SerializeField] private RoomGenerator1 generator;
    [SerializeField] private RoomTeleporter teleporter;

    [Header("설정")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private DirectionalRoomDatabaseSO roomDatabase;
    [SerializeField] private float roomWidth = 11f, roomHeight = 7f, tileOffset = 3.0f;

    private Dictionary<Vector2Int, RoomInfo> dungeonMap = new Dictionary<Vector2Int, RoomInfo>();
    private Dictionary<Vector2Int, Vector3> roomPositions = new Dictionary<Vector2Int, Vector3>();

    protected override void Awake()
    {
        base.Awake();
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
    // 플레이어 주변의 모든 방 중에서 거리가 가장 짧은 방을 찾아내어, 현재 플레이어가 속한 방이 무엇인지 확실하게 판별해주는 탐색기
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
        //카메라 설정
        var camFollow = Camera.main.GetComponent<CameraFollow>();
        if (camFollow != null) camFollow.enabled = false;
        isChangingRoom = true;

        //방 타입 결정
        RoomType nextRoomType = ruleChecker.DetermineNextRoomType(); // 보스/보물/일반 결정
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

            RoomInfo info = GetTargetRoomInfo(dir, nextRoomType);
            generator.CreateRoom(info, nextRoomPos, nextGridPos);

            // 중요: 생성된 방을 맵 데이터에 기록
            dungeonMap[nextGridPos] = info;
            roomPositions[nextGridPos] = nextRoomPos;
            Camera.main.transform.position = new Vector3(nextRoomPos.x, nextRoomPos.y, -10);
            teleporter.TeleportToDoor(player, doorSensor.transform.position, dir, tileOffset);

            ruleChecker.CurrentDoorUsedCount++;
            if (nextRoomType == RoomType.Normal)
            {
                ruleChecker.GeneratedNormalRoomCount++;
            }
        }
        StartCoroutine(ResetCameraLock(0.2f));
    }
    // 방 타입과 연결 방향에 따라 사용 가능한 방 리스트 중 하나를 무작위로 반환하는 함수
    private RoomInfo GetTargetRoomInfo(Vector2Int exitDir, RoomType type)
    {
        // 나가는 문의 반대 방향 (새로 생성될 방의 입구 문 방향)
        Vector2Int requiredEntryDir = -exitDir;
        DirectionalRoomSet set = roomDatabase.GetSetByType(type);

        //  검색할 리스트 결정 (해당 방향 전용 리스트 + 공용 리스트)
        List<RoomInfo> primaryList = requiredEntryDir switch
        {
            { x: 0, y: 1 } => set.up,
            { x: 0, y: -1 } => set.down,
            { x: -1, y: 0 } => set.left,
            { x: 1, y: 0 } => set.right,
            _=> new List<RoomInfo>()
        };
        //후보군을 통합 (전용 방 + Any 방)
        List<RoomInfo> candidates = new List<RoomInfo>(primaryList);
        candidates.AddRange(set.any);

        //문 위치가 맞는 방들만 최종 필터링
        List<RoomInfo> validRooms = new List<RoomInfo>();
        foreach (var room in candidates)
        {
            if (HasDoorInDirection(room.roomPrefab, requiredEntryDir))
            {
                if (!validRooms.Contains(room)) // 중복 방지
                    validRooms.Add(room);
            }
            else
            {
                // 디버깅용: 왜 탈락했는지 출력
                Debug.Log($"[필터링 탈락] 방: {room.roomName} / 요구 방향: {requiredEntryDir} / 문 없음");
            }
        }
        //최종 반환
        if (validRooms.Count > 0)
        {
            return validRooms[Random.Range(0, validRooms.Count)];
        }
        // 안전 장치: 문이 맞는 방이 하나도 없으면 시작 방 반환
        Debug.LogWarning($"[RoomManager] {requiredEntryDir} 방향에 문이 있는 방이 없습니다. 시작 방을 생성합니다.");
        return roomDatabase.startRoom;
    }
    private bool HasDoorInDirection(GameObject roomPrefab, Vector2Int dir)
    {
        foreach (var door in roomPrefab.GetComponentsInChildren<Doorinstall>())
        {
            if (door.WallDirection == dir) return true;
        }
        return false;
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
}