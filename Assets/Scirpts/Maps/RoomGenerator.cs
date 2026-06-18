using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private int maxRooms;
    private Dictionary<Vector2Int, RoomManager.RoomData> dungeonMap = new Dictionary<Vector2Int, RoomManager.RoomData>();
    private List<Vector2Int> roomPositions = new List<Vector2Int>();
    public RoomGenerator(int maxRooms)
    {
        this.maxRooms = maxRooms;
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
        List<Vector2Int> deadEnds = FindDeadEnds();
        AssignSpecialRooms(deadEnds);

        return dungeonMap;
    }
    private void RuleRoom()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // 일반 방 배치 (뭉침 방지 규칙 포함)
        while (roomPositions.Count < maxRooms)
        {
            Vector2Int randomStartPos = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2Int nextPos = randomStartPos + directions[Random.Range(0, directions.Length)];

            if (!dungeonMap.ContainsKey(nextPos))
            {
                if (GetNeighborCount(nextPos) >= 3) continue; // 뭉침 방지

                dungeonMap.Add(nextPos, new RoomManager.RoomData { gridPos = nextPos, type = RoomType.Normal });
                roomPositions.Add(nextPos);
            }
        }
    }

    private void AssignSpecialRooms(List<Vector2Int> deadEnds)
    {
        // 보스방 배정 (아래 기피 + 위에 방 없어야 함)
        if (deadEnds.Count > 0)
        {
            Vector2Int bossPos = GetFurthestDeadEndForBoss(Vector2Int.zero, deadEnds);
            dungeonMap[bossPos] = new RoomManager.RoomData { gridPos = bossPos, type = RoomType.Boss };
            deadEnds.Remove(bossPos);
        }
        else
        {
            Vector2Int fallbackPos = GetRandomNormalRoomPositionForBoss();
            if (fallbackPos != Vector2Int.zero)
                dungeonMap[fallbackPos] = new RoomManager.RoomData { gridPos = fallbackPos, type = RoomType.Boss };
        }

        // 보물방 배정
        AssignRemainingRoom(deadEnds, RoomType.Treasure);

        // 상점방 배정
        AssignRemainingRoom(deadEnds, RoomType.Store);
    }

    private void AssignRemainingRoom(List<Vector2Int> deadEnds, RoomType type)
    {
        if (deadEnds.Count > 0)
        {
            Vector2Int pos = GetFurthestDeadEnd(Vector2Int.zero, deadEnds);
            dungeonMap[pos] = new RoomManager.RoomData { gridPos = pos, type = type };
            deadEnds.Remove(pos);
        }
        else
        {
            Vector2Int fallbackPos = GetRandomNormalRoomPosition();
            if (fallbackPos != Vector2Int.zero)
                dungeonMap[fallbackPos] = new RoomManager.RoomData { gridPos = fallbackPos, type = type };
        }
    }

    //규칙 검사용 함수

    private int GetNeighborCount(Vector2Int pos)
    {
        int count = 0;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in directions)
        {
            if (dungeonMap.ContainsKey(pos + dir)) count++;
        }
        return count;
    }

    private List<Vector2Int> FindDeadEnds()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var pos in roomPositions)
        {
            if (pos == Vector2Int.zero) continue;
            int neighborCount = 0;
            foreach (var dir in directions)
            {
                if (dungeonMap.ContainsKey(pos + dir)) neighborCount++;
            }
            if (neighborCount == 1) deadEnds.Add(pos);
        }
        return deadEnds;
    }

    private Vector2Int GetFurthestDeadEnd(Vector2Int origin, List<Vector2Int> deadEnds)
    {
        Vector2Int furthest = deadEnds[0];
        float maxDistance = 0;
        foreach (var pos in deadEnds)
        {
            float distance = Mathf.Abs(pos.x - origin.x) + Mathf.Abs(pos.y - origin.y);
            if (distance > maxDistance) { maxDistance = distance; furthest = pos; }
        }
        return furthest;
    }

    private Vector2Int GetFurthestDeadEndForBoss(Vector2Int origin, List<Vector2Int> deadEnds)
    {
        Vector2Int furthest = deadEnds[0];
        float maxDistance = -9999f;
        foreach (var pos in deadEnds)
        {
            if (dungeonMap.ContainsKey(pos + Vector2Int.up)) continue; // 위에 방 있으면 차단

            float distance = Mathf.Abs(pos.x - origin.x) + Mathf.Abs(pos.y - origin.y);
            if (pos.y < 0) distance -= 30f; // 아래쪽 페널티

            if (distance > maxDistance) { maxDistance = distance; furthest = pos; }
        }
        return furthest;
    }

    private Vector2Int GetRandomNormalRoomPositionForBoss()
    {
        List<Vector2Int> validRooms = new List<Vector2Int>();
        foreach (var pair in dungeonMap)
        {
            if (pair.Value.type == RoomType.Normal && pair.Key.y >= 0 && !dungeonMap.ContainsKey(pair.Key + Vector2Int.up))
                validRooms.Add(pair.Key);
        }
        return validRooms.Count > 0 ? validRooms[Random.Range(0, validRooms.Count)] : GetRandomNormalRoomPosition();
    }

    private Vector2Int GetRandomNormalRoomPosition()
    {
        List<Vector2Int> normalRooms = new List<Vector2Int>();
        foreach (var pair in dungeonMap)
        {
            if (pair.Value.type == RoomType.Normal) normalRooms.Add(pair.Key);
        }
        return normalRooms.Count > 0 ? normalRooms[Random.Range(0, normalRooms.Count)] : Vector2Int.zero;
    }
}