using UnityEngine;

public class DynamicRoom : MonoBehaviour
{
    public Vector2Int RoomGridPos { get; private set; }
    public RoomInfo RoomConfig { get; private set; }
    public bool IsCleared { get; set; } = false;
    // RoomManager가 방을 생성한 후 좌표와 데이터 에셋을 주입하는 함수
    public void SetupRoom(Vector2Int gridPos, RoomInfo config)
    {
        this.RoomGridPos = gridPos;
        this.RoomConfig = config;
 
        // 에셋에 지정된 방 이름을 활용해 계층 구조 오브젝트명 세팅
        gameObject.name = $"Room_{gridPos.x}_{gridPos.y} ({config.roomName})";

        //테스트용 기본방 클리어 처리
        if (config.roomName == "시작방" || string.IsNullOrEmpty(config.roomName))
        {
            IsCleared = true;
        }
    }
}