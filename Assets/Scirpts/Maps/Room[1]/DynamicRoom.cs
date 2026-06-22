using UnityEngine;

public class DynamicRoom : MonoBehaviour
{
    public Vector2Int RoomGridPos { get; private set; }
    public RoomInfo RoomConfig { get; private set; }
    public bool IsCleared { get; set; } = false;
    // RoomManager가 방을 생성한 후 좌표와 데이터 에셋을 주입하는 함수
    public void SetupRoom(Vector2Int gridPos, RoomInfo info)
    {
        this.RoomGridPos = gridPos;

        // 이 부분이 정상적으로 작동하려면 위에서 추가한 속성이 필요합니다.
        Doorinstall[] doors = GetComponentsInChildren<Doorinstall>();
        foreach (var door in doors)
        {
            door.ParentRoomGridPos = gridPos; 
        }
    }
}