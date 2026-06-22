using UnityEngine;

public class RoomRuleChecker1
{
    private int minRoomsBeforeBoss;
    private int storeSpawnInterval;

    public int GeneratedNormalRoomCount { get; set; } = 0;
    public bool IsBossRoomSpawned { get; set; } = false;

    // 생성자를 통해 기획 설정값을 매니저로부터 넘겨받음
    public RoomRuleChecker1(int minRoomsBeforeBoss, int storeSpawnInterval)
    {
        this.minRoomsBeforeBoss = minRoomsBeforeBoss;
        this.storeSpawnInterval = storeSpawnInterval;
    }

    // 진행도와 이동 방향을 분석하여 다음 생성할 방의 타입을 결정합니다.
    public RoomType DetermineNextRoomType(Vector2Int direction)
    {
        if (IsBossRoomSpawned) return RoomType.Normal;

        // 보스방 조건 (최소 방 만족 및 위쪽 이동 시)
        if (GeneratedNormalRoomCount >= minRoomsBeforeBoss && direction == Vector2Int.up)
        {
            return RoomType.Boss;
        }

        //상점방 조건 (인터벌 만족 및 좌/우 이동 시)
        if (GeneratedNormalRoomCount > 0 && GeneratedNormalRoomCount % storeSpawnInterval == 0)
        {
            if (direction == Vector2Int.left || direction == Vector2Int.right)
            {
                return RoomType.Store;
            }
        }

        //보물방 예시 (3번째 방이고 위쪽 이동 시)
        if (GeneratedNormalRoomCount == 3 && direction == Vector2Int.up)
        {
            return RoomType.Treasure;
        }

        return RoomType.Normal;
    }
}