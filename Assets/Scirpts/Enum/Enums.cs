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
    Dead 
}

///////////////////////////////////////
//               Item
///////////////////////////////////////

public enum ItemType
{
    Equipment,          // 장비
    Consumable          // 소비
}

public enum EquipmentType
{ 
    Necklace,           // 장비창 1번
    Weapon,             // 장비창 2번
    Shield,             // 장비창 3번
    Ring                // 장비창 4번
}

public enum ConsumableType
{ 
    HpPotion            // Hp 회복 포션
}

///////////////////////////////////////
//                UI
///////////////////////////////////////

public enum VolumeType
{
    None = -1,
    Master,
    BGM,
    SFX
}

///////////////////////////////////////
//             SceneType
///////////////////////////////////////

public enum SceneType
{
    Title,
    InGame
}