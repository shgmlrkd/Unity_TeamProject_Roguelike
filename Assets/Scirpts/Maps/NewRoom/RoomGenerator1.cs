using UnityEngine;

public class RoomGenerator1 : MonoBehaviour
{
    // 방의 정보와 위치, 그리고 입장 방향을 받아 실제 게임 오브젝트로 생성합니다.
    public GameObject CreateRoom(RoomInfo roomInfo, Vector3 worldPos, Vector2Int gridPos, Vector2Int doorDirection)
    {
        GameObject roomObj = Instantiate(roomInfo.roomPrefab, worldPos, Quaternion.identity);

        if (roomObj.TryGetComponent<DynamicRoom>(out var comp))
        {
            comp.SetupRoom(gridPos, roomInfo);
        }
        return roomObj;
    }
    // 연결 방향 정보가 필요 없는 경우(시작 방 등)를 위한 오버로딩 함수
    // 기본적으로 방향을 Vector2Int.zero로 설정하여 위의 메인 함수를 호출
    public GameObject CreateRoom(RoomInfo roomInfo, Vector3 worldPos, Vector2Int gridPos)
    {
        return CreateRoom(roomInfo, worldPos, gridPos, Vector2Int.zero);
    }
}
