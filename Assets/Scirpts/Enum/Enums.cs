///////////////////////////////////////
//              Monster
///////////////////////////////////////
public enum MonsterStateEnum // 몬스터 상태
{
    None = -1,
    Idle, 
    Patrol, 
    Chase,
    Attack, 
    Hit,
    Dead
}

public enum NormalMonsterPattern // 일반 몬스터 패턴
{
    BaseAttack,
    PatternA,
    Length
}
public enum BossMonsterPattern // 보스 몬스터 패턴
{
    BaseAttack,
    BossPatternA,
    BossPatternB,
    Length
}


///////////////////////////////////////
//               Item
///////////////////////////////////////

public enum ItemType
{
    Equipment,          // 장비
    Consumable,         // 소비
    Gold,               // 골드
    Length              // 전체 길이
}

public enum EquipmentType
{ 
    Necklace,           // 장비창 1번
    Weapon,             // 장비창 2번
    Shield,             // 장비창 3번
    Ring,               // 장비창 4번
    Length              // 전체 길이
}

public enum ConsumableType
{ 
    HpPotion,           // Hp 회복 포션
    Length              // 전체 길이
}

///////////////////////////////////////
//                UI
///////////////////////////////////////

public enum VolumeType
{
    None = -1,
    Master,             // 마스터 볼륨
    BGM,                // BGM 볼륨
    SFX                 // SFX 볼륨
}

public enum HeartType 
{
    fullHeart,          // 체력이 가득 찬 하트
    halfHeart,          // 체력이 절반인 하트
    emptyHeart,         // 체력이 없는 빈 하트  
    lockHeart           // 쓸 수 없는 체력 (최대 체력이 랜덤이니 다 못 채울 수 있음)
}

public enum BonusType
{
    MoveSpeed,          // 이동 속도   
    Attack,             // 공격력
    AttackSpeed,        // 공격 속도
    Length
}

public enum GameOverTextType
{
    PlayTime,           // 플레이 시간
    BossClearCheck,     // 보스 처치 유무
    MonsterKillCount    // 몬스터 처치 수
}

///////////////////////////////////////
//              Sound
///////////////////////////////////////

public enum SoundKey
{
    // 씬 브금
    TitleBGM,
    InGameBGM,
    BoosRoomBGM,

    // 효과음
    PlayerFootStep,
    SwordSwing,
    CollectedItem,
    NormalDoorOpen,
    BossDoorOpen,
    ButtonClick,
    ButtonHover,
    PlayerHit,
    PlayerDead,
    MonsterHit,
    MonsterDead
}

///////////////////////////////////////
//             SceneType
///////////////////////////////////////

public enum SceneType
{
    Title,      // 타이틀 씬
    InGame      // 인게임 씬
}

///////////////////////////////////////
//              Map
///////////////////////////////////////
///
public enum RoomType
{ 
    None,
    Start,
    Normal,
    Boss, 
    Store,
    Treasure,
}

///////////////////////////////////////
//              Player
///////////////////////////////////////

public enum PlayerStateEnum
{
    None = -1,
    Idle,
    Move,
    Attack,
    Hit,
    Dead
}