using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<EquipmentType, EquipmentData> equipmentItems =
        new Dictionary<EquipmentType, EquipmentData>();

    public event Action<EquipmentType, EquipmentData> OnEquipmentStored;
    public event Action<EquipmentType> OnEquipmentRemoved;

    public void StoreEquipment(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        EquipmentType equipmentType = equipmentData.EquipmentType;

        equipmentItems[equipmentType] = equipmentData;

        OnEquipmentStored?.Invoke(equipmentType, equipmentData);

        //Debug.Log($"장비 저장: {equipmentType} / {equipmentData.ItemName}");
    }

    public bool TryGetEquipment(EquipmentType equipmentType, out EquipmentData equipmentData)
    {
        return equipmentItems.TryGetValue(equipmentType, out equipmentData);
    }

    public bool HasEquipment(EquipmentType equipmentType)
    {
        return equipmentItems.ContainsKey(equipmentType);
    }

    public void RemoveEquipment(EquipmentType equipmentType)
    {
        if (!equipmentItems.ContainsKey(equipmentType))
        {
            return;
        }

        equipmentItems.Remove(equipmentType);

        OnEquipmentRemoved?.Invoke(equipmentType);

        //Debug.Log($"장비 제거: {equipmentType}");
    }

    public void Clear()
    {
        equipmentItems.Clear();
    }
}