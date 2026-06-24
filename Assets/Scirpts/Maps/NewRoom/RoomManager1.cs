using Cainos.PixelArtTopDown_Basic;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class RoomManager1 : ScenesSingleton<RoomManager1>
{
    private DungeonMapController map = new DungeonMapController();
    private RoomBuilder builder;
    private bool isChangingRoom = false;

    [Header("모듈")]
    [SerializeField] private RoomGenerator1 generator;
    [SerializeField] private RoomTeleporter teleporter;
    [Header("설정")]
    [SerializeField] private DirectionalRoomDatabaseSO roomDatabase;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float roomWidth = 11f, roomHeight = 7f, tileOffset = 3.0f;
    [Header("카메라")]
    [SerializeField] private CameraRig cameraRig;

    protected override void Awake()
    {
        base.Awake();
        builder = new RoomBuilder(generator, roomDatabase);
    }

    private void Start()
    {
        generator.CreateRoom(roomDatabase.startRoom, playerTransform.position, Vector2Int.zero);
        map.DungeonMap[Vector2Int.zero] = roomDatabase.startRoom;
        map.RoomPositions[Vector2Int.zero] = playerTransform.position;
    }
    // 문을 통해 다른 방으로 이동하거나, 새로운 방을 생성하는 핵심 함수
    public void BuildRoomFromDoor(Doorinstall door, Transform player)
    {
        Vector2Int dir = door.WallDirection;
        Vector2Int nextGrid = door.ParentRoomGridPos + dir;

        if (map.DungeonMap.ContainsKey(nextGrid))
        {
            teleporter.TeleportToDoor(player, door.transform.position, dir, tileOffset);
        }
        else
        {
            RoomInfo info = builder.CreateRoom(dir, door.transform.position, nextGrid, roomWidth, roomHeight);
            map.DungeonMap[nextGrid] = info;
            map.RoomPositions[nextGrid] = door.transform.position + ((Vector3)(Vector2)dir * (dir.x != 0 ? roomWidth : roomHeight));
            teleporter.TeleportToDoor(player, door.transform.position, dir, tileOffset);
        }
        StartCoroutine(CloseDoorDelayed(door));
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
            cameraRig.MoveToRoom(pos, 0.5f, () => isChangingRoom = false);
    }
}