using System.Collections;
using UnityEngine;

public class RoomManager : ScenesSingleton<RoomManager>
{
    private DungeonMapController map = new DungeonMapController();
    private RoomBuilder builder;

    [Header("모듈")]
    [SerializeField] private RoomGenerator generator;
    [SerializeField] private RoomTeleporter teleporter;
    [Header("설정")]
    [SerializeField] private DirectionalRoomDatabaseSO roomDatabase;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float roomWidth = 11f, roomHeight = 7f, tileOffset = 3.0f, bossDoorTileOffset = 6.0f;
    [Header("카메라")]
    [SerializeField] private CameraRig cameraRig;

    protected override void Awake()
    {
        base.Awake();
        builder = new RoomBuilder(generator, roomDatabase);
    }

    private void Start()
    {
        if (generator == null || roomDatabase == null)
        {
            return;
        }
        if (RoomRuleChecker.Instance != null && RoomRuleChecker.Instance.CanGenerateMoreRooms)
        {
            generator.CreateRoom(roomDatabase.startRoom, playerTransform.position, Vector2Int.zero);
            map.DungeonMap[Vector2Int.zero] = roomDatabase.startRoom;
            map.RoomPositions[Vector2Int.zero] = playerTransform.position;
        }
    }
    // 문을 통해 다른 방으로 이동하거나, 새로운 방을 생성하는 핵심 함수
    public void BuildRoomFromDoor(Doorinstall door, Transform player)
    {
        if (RoomRuleChecker.Instance.IsInBossEntranceMode) return;

        Vector2Int dir = door.WallDirection;
        Vector2Int nextGrid = door.ParentRoomGridPos + dir;

        if (map.DungeonMap.ContainsKey(nextGrid))
        {
            ExecuteTeleport(player, door, dir);
        }
        else if (RoomRuleChecker.Instance.CanGenerateMoreRooms || door.isBossEntranceDoor)
        {
            RoomInfo info = builder.CreateRoom(dir, door.transform.position, nextGrid, roomWidth, roomHeight);
            if (info != null)
            {
                map.DungeonMap[nextGrid] = info;
                map.RoomPositions[nextGrid] = door.transform.position + ((Vector3)(Vector2)dir * (dir.x != 0 ? roomWidth : roomHeight));
                ExecuteTeleport(player, door, dir);
            }
        }
        StartCoroutine(CloseDoorDelayed(door));
        MoveCamera();
    }
    public void SpawnBossRoomOnly(Doorinstall door)
    {
        Vector2Int nextGrid = door.ParentRoomGridPos + door.WallDirection;

        // 이미 생성되어 있다면 중복 생성 방지
        if (map.DungeonMap.ContainsKey(nextGrid)) return;

        // 보스 방 위치 계산
        Vector3 spawnPos = door.transform.position + ((Vector3)(Vector2)door.WallDirection * 15f);

        // 보스 방 프리팹 생성 (RoomBuilder를 거치지 않고 generator에 직접 명령)
        generator.CreateRoom(roomDatabase.bossRoom, spawnPos, nextGrid, door.WallDirection);

        // 맵 데이터에 등록 (나중에 텔레포트가 작동하도록 함)
        map.DungeonMap[nextGrid] = roomDatabase.bossRoom;
        map.RoomPositions[nextGrid] = spawnPos;
    }
    private void ExecuteTeleport(Transform player, Doorinstall door, Vector2Int dir)
    {
        Vector2Int nextGrid = door.ParentRoomGridPos + dir;

        if(!RoomRuleChecker.Instance.CanGenerateMoreRooms)
        {
            teleporter.TeleportToDoor(player, door.transform.position, dir, bossDoorTileOffset);
            MoveCamera();
            return;
        }

        // 방 데이터가 실제로 있는지 한 번 더 확인 (안전장치)
        if (teleporter.CanTeleport(nextGrid, map.DungeonMap))
        {
            teleporter.TeleportToDoor(player, door.transform.position, dir, tileOffset);
            MoveCamera();
        }
    }
    public void ForceMoveToBossRoom(Vector3 pos, Vector2Int grid)
    {

        // 1. 보스방 생성
        generator.CreateRoom(roomDatabase.bossRoom, pos, grid, Vector2Int.zero);

        // 2. 맵 데이터 저장
        map.DungeonMap[grid] = roomDatabase.bossRoom;
        map.RoomPositions[grid] = pos;

        // 3. 순간이동 및 카메라 이동
        playerTransform.position = pos;
        RoomRuleChecker.Instance.IsInBossEntranceMode = true;
        MoveCamera();
    }
    // 문을 통과한 후 플레이어가 방에서 완전히 벗어날 시간을 벌고 문을 닫는 함수
    private IEnumerator CloseDoorDelayed(Doorinstall door)
    {
        yield return new WaitForSeconds(0.5f);
        door.CloseDoor();
    }
    // 플레이어의 현재 위치(그리드)를 파악하여 해당 방의 위치로 카메라를 이동시키는 함수
    private void MoveCamera()
    {
        Vector2Int grid = map.GetCurrentRoomGridPos(playerTransform.position);
        
        if (map.RoomPositions.TryGetValue(grid, out Vector3 pos))
        { 
            cameraRig.MoveToRoom(pos, 0.5f); 
        }
    }
}