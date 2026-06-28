using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EquipmentGroup
{
    public EquipmentType type;
    public List<EquipmentData> datas;
}

[System.Serializable]
public class ConsumableGroup
{
    public ConsumableType type;
    public List<ConsumableData> datas;
}

[System.Serializable]
public class Gold
{
    public ItemData Data;
}

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "GameDataBase/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField]
    private List<EquipmentGroup> equipmentGroup;

    [SerializeField]
    private List<ConsumableGroup> consumableGroup;

    [SerializeField]
    private Gold gold;

    // 장비 아이템 타입 그룹에서 랜덤 장비 아이템 데이터를 반환
    public EquipmentData GetEquipmentData(EquipmentType key)
    {
        EquipmentGroup group = equipmentGroup.FirstOrDefault(g => g.type == key);

        if (group == null || group.datas.Count == 0)
            return null;

        return group.datas[Random.Range(0, group.datas.Count)];
    }

    // 소비 아이템 타입 그룹에서 랜덤 소비 아이템 데이터를 반환
    public ConsumableData GetConsumableData(ConsumableType key)
    {
        ConsumableGroup group = consumableGroup.FirstOrDefault(g => g.type == key);

        if (group == null || group.datas.Count == 0)
            return null;

        return group.datas[Random.Range(0, group.datas.Count)];
    }

    // 골드 아이템 데이터를 반환
    public ItemData GetGoldData()
    {
        return gold.Data;
    }
}