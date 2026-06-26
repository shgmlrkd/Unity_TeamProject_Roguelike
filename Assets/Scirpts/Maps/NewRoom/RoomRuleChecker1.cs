using UnityEngine;

public class RoomRuleChecker1
{
    public int GeneratedNormalRoomCount { get; set; } = 0;
    public int MaxDoorCount { get; set; } = 10; // 보스 방 등장을 위한 제한 횟수
    public int CurrentDoorUsedCount { get; set; } = 0;

    // 진행도와 이동 방향을 분석하여 다음 생성할 방의 타입을 결정합니다.
    public RoomType DetermineNextRoomType()
    {
        //보스방 조건
        if (IsTimeForBossRoom()) return RoomType.Boss;

        //보물방 조건
        if (GeneratedNormalRoomCount >= 4)
        {
            float treasureChance = 0.1f + (GeneratedNormalRoomCount * 0.05f); // 10% 시작, 방마다 5% 증가
            if (Random.value < treasureChance) return RoomType.Treasure;
        }
        //그외 모두 일반방
        return RoomType.Normal;
    }
    public bool IsTimeForBossRoom()
    {
        // 정해진 문 통과 횟수(MaxDoorCount)에 도달했는지 확인
        return CurrentDoorUsedCount >= MaxDoorCount;
    }
}
