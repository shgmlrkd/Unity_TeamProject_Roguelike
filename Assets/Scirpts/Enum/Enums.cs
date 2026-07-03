///////////////////////////////////////
//              Monster
///////////////////////////////////////
public enum MonsterStateEnum
{
    None = -1,
    Idle, 
    Patrol, 
    Chase,
    Attack, 
    Hit,
    Dead
}

public enum NormalMonsterPattern
{
    BaseAttack,
    Length
}

public enum BossStateEnum
{
    None = -1,
    Idle,               // 대기
    Chase,              // 추격
    AttackSelect,       // 공격 선택
    BaseAttack,         // 기본 공격
    DashAttack,         // 돌진 공격
    Summon,             // 잡몹 소환
    PhaseTransition,    // 회복 (2페이즈)
    ProjectileAttack,   // 투사체 공격
    Dead,               // 죽음
    Hit                 // 다침
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
    #region BGM

    TitleBGM,
    InGameBGM,
    BoosRoomBGM,

    #endregion BGM

    #region SFX

    // Player
    SwordSwing,             // 공격 소리
    PlayerFootStep,         // 걷는 소리
    PlayerHit,              // 맞는 소리
    PlayerDead,             // 죽는 소리

    // Monster
    MonsterHit,             // 맞는 소리
    MonsterDead,            // 죽는 소리

    // UI
    BossDoorOpen,           // 보스 방 열리는 소리
    ButtonHover,            // 버튼 위에 마우스 올릴 때 나는 소리
    ButtonClick,            // 버튼 클릭 소리
    CollectedItem,          // 아이템 먹는 소리
    DoorOpen,               // 일반 문 열리는 소리
    MapClear,               // 맵의 몬스터 다 잡았을 때 소리

    // Boss
    BossAxAttack,           // 보스 공격 소리
    BossRun,                // 보스 발 소리
    BossDashIntro,          // 보스 대쉬 공격 준비 소리
    BossDashStart,          // 보스 대쉬 돌진 소리
    BossCrash,              // 플레이어나 벽에 부딪힌 소리
    BossSpawnMonster,       // 일반 몹 소환 소리
    BossPhaseChange,        // 페이즈 넘어가는 소리
    BossDeadIntro,          // 보스 죽는 소리 (울음)
    BossDead,               // 보스 죽는 소리 (털썩)
    BossFireBall,           // 보스 투사체 소리
    #endregion SFX
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