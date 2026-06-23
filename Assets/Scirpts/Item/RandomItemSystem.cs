using UnityEngine;

public class RandomItemSystem
{
    private ItemDataBase itemDataBase;

    public RandomItemSystem(ItemDataBase itemDataBase)
    {
        this.itemDataBase = itemDataBase;
    }

    public ItemData GetRandomItem()
    {
        // 이건 장비 타입인지 소비 타입인지 랜덤 돌린거
        int itemType = 0;// Random.Range(0, (int)ItemType.Length);

        switch ((ItemType)itemType)
        {
            // 장비
            case ItemType.Equipment:
                EquipmentType equipmentTypeKey =
                    (EquipmentType)Random.Range(0, (int)EquipmentType.Length);

                return itemDataBase.GetEquipmentData(equipmentTypeKey);
            
            // 소비
            case ItemType.Consumable:
                ConsumableType consumableTypeKey =
                    (ConsumableType)Random.Range(0, (int)ConsumableType.Length);

                return itemDataBase.GetConsumableData(consumableTypeKey);
        }

        return null;
    }
}
