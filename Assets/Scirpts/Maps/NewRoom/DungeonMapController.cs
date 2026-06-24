using System.Collections.Generic;
using UnityEngine;

public class DungeonMapController
{
    public Dictionary<Vector2Int, RoomInfo> DungeonMap = new Dictionary<Vector2Int, RoomInfo>();
    public Dictionary<Vector2Int, Vector3> RoomPositions = new Dictionary<Vector2Int, Vector3>();

    // 현재 플레이어의 월드 좌표를 받아, 가장 가까운 방의 그리드 좌표(Vector2Int)를 반환
    public Vector2Int GetCurrentRoomGridPos(Vector3 playerPos)
    {
        Vector2Int bestGrid = Vector2Int.zero;
        float minDistance = float.MaxValue;

        foreach (var kvp in RoomPositions)
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
}
