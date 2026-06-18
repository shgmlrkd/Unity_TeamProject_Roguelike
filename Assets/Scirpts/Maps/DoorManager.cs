using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [System.Serializable]
    private struct DoorData
    {
        public bool hasUp;
        public bool hasDown;
        public bool hasLeft;
        public bool hasRight;
    }

    [Header("문 프리팹 설정")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private float doorOffsetFromCenter = 10f;

    // 각 좌표별 방의 문 장부를 내부적으로 안전하게 격리 보관
    private Dictionary<Vector2Int, DoorData> doorMap = new Dictionary<Vector2Int, DoorData>();

    public void CalculateDoors(Dictionary<Vector2Int, RoomManager.RoomData> dungeonMap)
    {
        doorMap.Clear();
        List<Vector2Int> keys = new List<Vector2Int>(dungeonMap.Keys);

        foreach (Vector2Int pos in keys)
        {
            RoomManager.RoomData room = dungeonMap[pos];
            DoorData door = new DoorData();

            bool upPossible = dungeonMap.ContainsKey(pos + Vector2Int.up);
            bool downPossible = dungeonMap.ContainsKey(pos + Vector2Int.down);
            bool leftPossible = dungeonMap.ContainsKey(pos + Vector2Int.left);
            bool rightPossible = dungeonMap.ContainsKey(pos + Vector2Int.right);

            // 기획 규칙 적용
            switch (room.type)
            {
                case RoomType.Start:
                    door.hasLeft = leftPossible; door.hasRight = rightPossible; break;
                case RoomType.Boss:
                    door.hasDown = downPossible; break;
                case RoomType.Store:
                    door.hasLeft = leftPossible; door.hasRight = rightPossible; break;
                case RoomType.Treasure:
                    door.hasDown = downPossible; door.hasLeft = leftPossible; door.hasRight = rightPossible; break;
                case RoomType.Normal:
                    door.hasUp = upPossible; door.hasDown = downPossible; door.hasLeft = leftPossible; door.hasRight = rightPossible; break;
            }
            doorMap[pos] = door;
        }

        // 문 상호 동기화 (한쪽만 벽이 되는 버그 방지)
        foreach (Vector2Int pos in keys)
        {
            DoorData door = doorMap[pos];
            if (!door.hasUp && doorMap.ContainsKey(pos + Vector2Int.up)) { var n = doorMap[pos + Vector2Int.up]; n.hasDown = false; doorMap[pos + Vector2Int.up] = n; }
            if (!door.hasDown && doorMap.ContainsKey(pos + Vector2Int.down)) { var n = doorMap[pos + Vector2Int.down]; n.hasUp = false; doorMap[pos + Vector2Int.down] = n; }
            if (!door.hasLeft && doorMap.ContainsKey(pos + Vector2Int.left)) { var n = doorMap[pos + Vector2Int.left]; n.hasRight = false; doorMap[pos + Vector2Int.left] = n; }
            if (!door.hasRight && doorMap.ContainsKey(pos + Vector2Int.right)) { var n = doorMap[pos + Vector2Int.right]; n.hasLeft = false; doorMap[pos + Vector2Int.right] = n; }
        }
    }

  
    public void SpawnDoorsInRoom(GameObject roomObject, Vector2Int gridPos, float roomWidth, float roomHeight)
    {
        if (!doorMap.ContainsKey(gridPos)) return;

        DoorData data = doorMap[gridPos];
        Vector3 roomCenter = roomObject.transform.position;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        bool[] hasDoors = { data.hasUp, data.hasDown, data.hasLeft, data.hasRight };

        for (int i = 0; i < dirs.Length; i++)
        {
            if (hasDoors[i])
            {
                Vector3 posOffset = new Vector3(dirs[i].x * doorOffsetFromCenter * (roomWidth / roomHeight), dirs[i].y * doorOffsetFromCenter, 0);
                if (dirs[i].y != 0) posOffset.x = 0;

                GameObject doorObj = Instantiate(doorPrefab, roomCenter + posOffset, Quaternion.identity, roomObject.transform);
                doorObj.name = $"Door_{dirs[i]}";

                if (doorObj.TryGetComponent<Door>(out Door doorComponent))
                {
                    doorComponent.doorDirection = dirs[i];
                }
            }
        }
    }
}
