using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder
{
    private RoomGenerator1 generator;
    private DirectionalRoomDatabaseSO database;
    private RoomInfo lastRoom;

    public RoomBuilder(RoomGenerator1 gen, DirectionalRoomDatabaseSO db)
    {
        generator = gen;
        database = db;
    }
    // 새로운 방을 결정하고 배치
    public RoomInfo CreateRoom(Vector2Int dir, Vector3 doorPos, Vector2Int nextGrid, float width, float height)
    {
        RoomType nextType = RoomRuleChecker1.Instance.IsTimeForBossRoom() ? RoomType.Boss : RoomRuleChecker1.Instance.DetermineNextRoomType();
        if (nextType == RoomType.None) return null;

        Vector3 nextPos = doorPos + ((Vector3)(Vector2)dir * (dir.x != 0 ? width : height));
        RoomInfo info = GetTargetRoomInfo(dir, nextType);
        generator.CreateRoom(info, nextPos, nextGrid, dir);

        if (nextType == RoomType.Boss) RoomRuleChecker1.Instance.CanGenerateMoreRooms = false;

        if (nextType == RoomType.Normal) RoomRuleChecker1.Instance.GeneratedNormalRoomCount++;
        RoomRuleChecker1.Instance.CurrentDoorUsedCount++;

        return info;
    }
    // 생성할 방 타입과 연결 방향을 고려하여, 사용 가능한 방 후보군 중 하나를 반환
    private RoomInfo GetTargetRoomInfo(Vector2Int exitDir, RoomType type)
    {
        Vector2Int entryDir = -exitDir;
        DirectionalRoomSet set = database.GetSetByType(type);
        List<RoomInfo> candidates = new List<RoomInfo>(entryDir switch
        {
            { x: 0, y: 1 } => set.up, { x: 0, y: -1 } => set.down,
            { x: -1, y: 0 } => set.left, { x: 1, y: 0 } => set.right,
            _ => new List<RoomInfo>()
        });
        candidates.AddRange(set.any);

        List<RoomInfo> validRooms = new List<RoomInfo>();
        foreach (var room in candidates)
        {
            if (HasDoorInDirection(room.roomPrefab, entryDir) && room != lastRoom)
            {
                validRooms.Add(room);
            }
        }

        if (validRooms.Count > 0)
        {
            lastRoom = validRooms[Random.Range(0, validRooms.Count)]; // 선택된 방 저장
            return lastRoom;
        }

        return database.startRoom;
    }
    // 특정 프리팹 내부에 지정한 방향을 향하는 문(Doorinstall)이 있는지 확인
    private bool HasDoorInDirection(GameObject prefab, Vector2Int dir)
    {
        foreach (var door in prefab.GetComponentsInChildren<Doorinstall>())
            if (door.WallDirection == dir) return true;
        return false;
    }

}