using System.Collections.Generic;
using UnityEngine;
public class RoomRuleChecker
{
    private Dictionary<Vector2Int, RoomManager.RoomData> dungeonMap;
    private List<Vector2Int> roomPositions;
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    // 생성자: 검사할 지도(dungeonMap)와 좌표 목록을 주입받음
    public RoomRuleChecker(Dictionary<Vector2Int, RoomManager.RoomData> dungeonMap, List<Vector2Int> roomPositions)
    {
        this.dungeonMap = dungeonMap;
        this.roomPositions = roomPositions;
    }

 
    // 특정 좌표 주변에 이웃한 방이 몇 개인지 카운트 (뭉침 방지용)
    public int GetNeighborCount(Vector2Int pos)
    {
        int count = 0;
        foreach (var dir in directions)
        {
            if (dungeonMap.ContainsKey(pos + dir)) count++;
        }
        return count;
    }

    // 사방 중 단 한 곳만 연결된 막다른 길(외톨이 방) 목록 추출
    public List<Vector2Int> FindDeadEnds()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var pos in roomPositions)
        {
            if (pos == Vector2Int.zero) continue; // 시작 방(0,0)은 사방이 막혔어도 제외

            int neighborCount = GetNeighborCount(pos);
            if (neighborCount == 1) deadEnds.Add(pos);
        }
        return deadEnds;
    }

    // 일반 특수방(보물, 상점)용: 기점(origin)에서 단순 맨하탄 거리가 가장 먼 막다른 길 검색
    public Vector2Int GetFurthestDeadEnd(Vector2Int origin, List<Vector2Int> deadEnds)
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

    // 보스방용: 거리가 멀면서 [위에 방이 없고], [아래쪽 생성을 피하는] 최적의 막다른 길 검색
    public Vector2Int GetFurthestDeadEndForBoss(Vector2Int origin, List<Vector2Int> deadEnds)
    {
        Vector2Int furthest = deadEnds[0];
        float maxDistance = -9999f;

        foreach (var pos in deadEnds)
        {
            // 규칙 1: 보스방 바로 위(Up)에 다른 방이 존재하면 후보 탈락
            if (dungeonMap.ContainsKey(pos + Vector2Int.up)) continue;

            float distance = Mathf.Abs(pos.x - origin.x) + Mathf.Abs(pos.y - origin.y);

            // 규칙 2: Y축이 0 미만(아래쪽 영역)이면 감점 패널티 부여
            if (pos.y < 0) distance -= 30f;

            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthest = pos;
            }
        }
        return furthest;
    }
    // 막다른 길이 없을 때, 보스방 조건(Y>=0 및 위에 방 없음)을 충족하는 일반 방 중 하나를 무작위 선택
    public Vector2Int GetRandomNormalRoomPositionForBoss()
    {
        List<Vector2Int> validRooms = new List<Vector2Int>();
        foreach (var pair in dungeonMap)
        {
            if (pair.Value.type == RoomType.Normal && pair.Key.y >= 0 && !dungeonMap.ContainsKey(pair.Key + Vector2Int.up))
            {
                validRooms.Add(pair.Key);
            }
        }
        return validRooms.Count > 0 ? validRooms[Random.Range(0, validRooms.Count)] : GetRandomNormalRoomPosition();
    }

    // 순수 일반 방(Normal) 목록 중 하나를 완전히 무작위로 반환 (최종 예외 처리용)
    public Vector2Int GetRandomNormalRoomPosition()
    {
        List<Vector2Int> normalRooms = new List<Vector2Int>();
        foreach (var pair in dungeonMap)
        {
            if (pair.Value.type == RoomType.Normal) normalRooms.Add(pair.Key);
        }
        return normalRooms.Count > 0 ? normalRooms[Random.Range(0, normalRooms.Count)] : Vector2Int.zero;
    }
}