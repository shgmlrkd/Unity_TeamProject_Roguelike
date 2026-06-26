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
    emptyHeart          // 체력이 없는 빈 하트  
}

public enum BonusType
{
    MoveSpeed,          // 이동 속도   
    Attack,             // 공격력
    AttackSpeed,        // 공격 속도
    Length
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