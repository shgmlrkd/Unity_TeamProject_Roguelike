using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    private int maxRooms;
    private Dictionary<Vector2Int, RoomManager.RoomData> dungeonMap = new Dictionary<Vector2Int, RoomManager.RoomData>();
    private List<Vector2Int> roomPositions = new List<Vector2Int>();

    private RoomRuleChecker checker;
    public RoomGenerator(int maxRooms)
    {
        this.maxRooms = maxRooms;
        this.checker = new RoomRuleChecker(dungeonMap, roomPositions);
    }
    public Dictionary<Vector2Int, RoomManager.RoomData> Generate()
    {
        dungeonMap.Clear();
        roomPositions.Clear();

        // 시작 방 배치
        Vector2Int currentPos = Vector2Int.zero;
        dungeonMap.Add(currentPos, new RoomManager.RoomData { gridPos = currentPos, type = RoomType.Start });
        roomPositions.Add(currentPos);

        // 일반 방 배치 (뭉침 방지 규칙 포함)
        RuleRoom();

        // 특수방 배정
        List<Vector2Int> deadEnds = checker.FindDeadEnds();
        AssignSpecialRooms(deadEnds);

        return dungeonMap;
    }
    // 일반 방 배치 (뭉침 방지 규칙 포함)
    private void RuleRoom()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (roomPositions.Count < maxRooms)
        {
            Vector2Int randomStartPos = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2Int nextPos = randomStartPos + directions[Random.Range(0, directions.Length)];

            if (!dungeonMap.ContainsKey(nextPos))
            {
                if (checker.GetNeighborCount(nextPos) >= 3) continue; // 뭉침 방지

                dungeonMap.Add(nextPos, new RoomManager.RoomData { gridPos = nextPos, type = RoomType.Normal });
                roomPositions.Add(nextPos);
            }
        }
    }
    // 맵 생성이 끝난 후 막다른 길 목록을 받아 특수방을 배정하는 함수
    private void AssignSpecialRooms(List<Vector2Int> deadEnds)
    {
        // 보스방 배정 (아래 기피 + 위에 방 없어야 함)
        if (deadEnds.Count > 0)
        {
            Vector2Int bossPos = checker.GetFurthestDeadEndForBoss(Vector2Int.zero, deadEnds);
            dungeonMap[bossPos] = new RoomManager.RoomData { gridPos = bossPos, type = RoomType.Boss };
            deadEnds.Remove(bossPos);
        }
        else
        {
            Vector2Int fallbackPos = checker.GetRandomNormalRoomPositionForBoss();
            if (fallbackPos != Vector2Int.zero)
                dungeonMap[fallbackPos] = new RoomManager.RoomData { gridPos = fallbackPos, type = RoomType.Boss };
        }

        // 보물방 배정
        AssignRemainingRoom(deadEnds, RoomType.Treasure);

        // 상점방 배정
        AssignRemainingRoom(deadEnds, RoomType.Store);
    }

    // 보물방, 상점방처럼 남은 막다른 길에 순차적으로 방 종류를 덮어씌우는 유틸 함수
    private void AssignRemainingRoom(List<Vector2Int> deadEnds, RoomType type)
    {
        if (deadEnds.Count > 0)
        {
            Vector2Int pos = checker.GetFurthestDeadEnd(Vector2Int.zero, deadEnds);
            dungeonMap[pos] = new RoomManager.RoomData { gridPos = pos, type = type };
            deadEnds.Remove(pos);
        }
        else
        {
            Vector2Int fallbackPos = checker.GetRandomNormalRoomPosition();
            if (fallbackPos != Vector2Int.zero)
                dungeonMap[fallbackPos] = new RoomManager.RoomData { gridPos = fallbackPos, type = type };
        }
    }
}
    