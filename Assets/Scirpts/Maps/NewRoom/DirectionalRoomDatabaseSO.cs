using System.Collections.Generic;
using UnityEngine;
using static RoomManager;
[System.Serializable]
public struct RoomInfo
{
    public string roomName;  // 방 이름 
    public GameObject roomPrefab; // 실제 배치될 방 프리팹
}
[CreateAssetMenu(fileName = "DirectionalRoomDatabase", menuName = "Dungeon/Directional Room Database")]
public class DirectionalRoomDatabaseSO : ScriptableObject
{
    [Header("시작 방 세팅")]
    public RoomInfo startRoom;

    [Header("방향 상관없는 공용 일반 방 그룹")]
    public List<RoomInfo> normalRooms_Any = new List<RoomInfo>();

    [Header("방향별 일반 전투 방 그룹")]
    public List<RoomInfo> normalRooms_Up = new List<RoomInfo>();
    public List<RoomInfo> normalRooms_Down = new List<RoomInfo>();
    public List<RoomInfo> normalRooms_Left = new List<RoomInfo>();
    public List<RoomInfo> normalRooms_Right = new List<RoomInfo>();

    [Header("방향별 보스 방 그룹")]
    public List<RoomInfo> bossRooms_Up = new List<RoomInfo>();
    public List<RoomInfo> bossRooms_Down = new List<RoomInfo>();
    public List<RoomInfo> bossRooms_Left = new List<RoomInfo>();
    public List<RoomInfo> bossRooms_Right = new List<RoomInfo>();

    [Header("방향별 상점 방 그룹")]
    public List<RoomInfo> storeRooms_Up = new List<RoomInfo>();
    public List<RoomInfo> storeRooms_Down = new List<RoomInfo>();
    public List<RoomInfo> storeRooms_Left = new List<RoomInfo>();
    public List<RoomInfo> storeRooms_Right = new List<RoomInfo>();

    [Header("방향별 보물 방 그룹")]
    public List<RoomInfo> treasureRooms_Up = new List<RoomInfo>();
    public List<RoomInfo> treasureRooms_Down = new List<RoomInfo>();
    public List<RoomInfo> treasureRooms_Left = new List<RoomInfo>();
    public List<RoomInfo> treasureRooms_Right = new List<RoomInfo>();
}
