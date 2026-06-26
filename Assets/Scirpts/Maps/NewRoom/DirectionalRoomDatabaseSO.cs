using System.Collections.Generic;
using UnityEngine;
using static RoomManager;
[System.Serializable]
public class RoomInfo
{
    public string roomName;  // 방 이름 
    public GameObject roomPrefab; // 실제 배치될 방 프리팹
    public RoomType type;
}
[System.Serializable]
public class DirectionalRoomSet
{
    public List<RoomInfo> up = new List<RoomInfo>();
    public List<RoomInfo> down = new List<RoomInfo>();
    public List<RoomInfo> left = new List<RoomInfo>();
    public List<RoomInfo> right = new List<RoomInfo>();
    public List<RoomInfo> any = new List<RoomInfo>();
}
[CreateAssetMenu(fileName = "DirectionalRoomDatabase", menuName = "Dungeon/Directional Room Database")]
public class DirectionalRoomDatabaseSO : ScriptableObject
{
    public RoomInfo startRoom;
    public RoomInfo bossRoom;

    [Header("일반 방 (방향별 분류)")]
    public DirectionalRoomSet normalRooms;

    [Header("특수 방 (방향별 분류)")]
    public DirectionalRoomSet bossEntranceRoom;
    public DirectionalRoomSet treasureRooms;
    public DirectionalRoomSet storeRooms;

    // 데이터 가져오기 편의 함수
    public DirectionalRoomSet GetSetByType(RoomType type) => type switch
    {
        RoomType.Boss => bossEntranceRoom,
        RoomType.Treasure => treasureRooms,
        RoomType.Store => storeRooms,
        _ => normalRooms
    };
}
