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

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "GameDataBase/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField]
    private List<EquipmentGroup> equipmentGroup;

    [SerializeField]
    private List<ConsumableGroup> consumableGroup;

    public EquipmentData GetEquipmentData(EquipmentType key)
    {
        EquipmentGroup group = equipmentGroup.FirstOrDefault(g => g.type == key);

        if (group == null || group.datas.Count == 0)
            return null;

        return group.datas[Random.Range(0, group.datas.Count)];
    }

    public ConsumableData GetConsumableData(ConsumableType key)
    {
        ConsumableGroup group = consumableGroup.FirstOrDefault(g => g.type == key);

        if (group == null || group.datas.Count == 0)
            return null;

        return group.datas[Random.Range(0, group.datas.Count)];
    }
}