using UnityEngine;

public class RandomItemSystem
{
    private const int EquipmentDropChance = 20;
    private const int ConsumableDropChance = 10;

    private ItemDataBase itemDataBase;

    public RandomItemSystem(ItemDataBase itemDataBase)
    {
        this.itemDataBase = itemDataBase;
    }

    // 아이템 카운트가 2개이상이면 아이템 드랍
    public ItemData GetRandomItem()
    {
        int random = Random.Range(0, 100);

        if (random < EquipmentDropChance)
        {
            EquipmentType equipmentTypeKey =
                (EquipmentType)Random.Range(0, (int)EquipmentType.Length);

            return itemDataBase.GetEquipmentData(equipmentTypeKey);
        }

        if (random < EquipmentDropChance + ConsumableDropChance)
        {
            ConsumableType consumableTypeKey =
                (ConsumableType)Random.Range(0, (int)ConsumableType.Length);

            return itemDataBase.GetConsumableData(consumableTypeKey);
        }

        return null;
    }

    // 아이템 카운트가 1개면 골드만 나옴
    public ItemData GetGoldItem()
    {
        return itemDataBase.GetGoldData();
    }
}