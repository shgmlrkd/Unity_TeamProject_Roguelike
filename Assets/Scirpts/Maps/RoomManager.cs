using System.Collections.Generic;
using UnityEngine;
public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public struct RoomData
    {
        public Vector2Int gridPos;
        public RoomType type;
    }

    [Header("맵 생성 설정")]
    [SerializeField] private int maxRooms = 15;
    [SerializeField] public float roomWidth = 32;
    [SerializeField] public float roomHeight = 21;

    [Header("방 프리팹")]
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private List<GameObject> normalRoomPrefabs;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private GameObject storeRoomPrefab;
    [SerializeField] private GameObject treasureRoomPrefab;

    private DoorManager doorManager;
    private void Awake()
    {
        doorManager = FindFirstObjectByType<DoorManager>();
    }
    private void Start()
    {
        GenerateAndSpawnDungeon();
    }
    private void GenerateAndSpawnDungeon()
    {
        RoomGenerator generator = new RoomGenerator(maxRooms);
        Dictionary<Vector2Int, RoomData> dungeonMap = generator.Generate();
        if (doorManager != null)
        {
            doorManager.CalculateDoors(dungeonMap);
        }
        SpawnRoomPrefabs(dungeonMap);
    }

    private void SpawnRoomPrefabs(Dictionary<Vector2Int, RoomData> dungeonMap)
    {
        foreach (var pair in dungeonMap)
        {
            RoomData room = pair.Value;
            Vector3 worldPos = new Vector3(room.gridPos.x * roomWidth, room.gridPos.y * roomHeight, 0);
            GameObject spawnedRoom = null;

            switch (room.type)
            {
                case RoomType.Start:
                    Instantiate(startRoomPrefab, worldPos, Quaternion.identity);
                    break;
                case RoomType.Boss:
                    Instantiate(bossRoomPrefab, worldPos, Quaternion.identity);
                    break;
                case RoomType.Treasure:
                    Instantiate(treasureRoomPrefab, worldPos, Quaternion.identity);
                    break;
                case RoomType.Store:
                    Instantiate(storeRoomPrefab, worldPos, Quaternion.identity);
                    break;
                case RoomType.Normal:
                    GameObject randomNormalPrefab = normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Count)];
                    Instantiate(randomNormalPrefab, worldPos, Quaternion.identity);
                    break;
            }
            if (spawnedRoom != null && doorManager != null)
            {
                doorManager.SpawnDoorsInRoom(spawnedRoom, room.gridPos, roomWidth, roomHeight);
            }
        }
    }
}