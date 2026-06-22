using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    public void TeleportToDoor(Transform player, Vector3 doorPos, Vector2Int direction, float offset)
    {
        // 문 위치에서 방향대로 살짝 밀어내어 벽에 끼이지 않게 함
        player.position = doorPos + ((Vector3)(Vector2)direction * offset);
    }
}
