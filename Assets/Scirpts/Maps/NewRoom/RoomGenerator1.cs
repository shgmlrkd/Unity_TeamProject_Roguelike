using UnityEngine;

public class RoomGenerator1 : MonoBehaviour
{
    public GameObject CreateRoom(RoomInfo roomInfo, Vector3 worldPos, Vector2Int gridPos)
    {
        // 자식 좌표를 수정하는 루프를 삭제했습니다.
        GameObject roomObj = Instantiate(roomInfo.roomPrefab, worldPos, Quaternion.identity);

        if (roomObj.TryGetComponent<DynamicRoom>(out var comp))
        {
            comp.SetupRoom(gridPos, roomInfo);
        }
        return roomObj;
    }
}
