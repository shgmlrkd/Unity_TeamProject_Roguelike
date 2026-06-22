using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<EquipmentType, EquipmentData> equipmentItems =
        new Dictionary<EquipmentType, EquipmentData>();

    // 이 부분 수정 했습니다 - 노희강 -
    // EquipmentType 대신 Vector3 (위치)
    public event Action<Vector3, EquipmentData> OnEquipmentStored;
    public event Action<EquipmentType> OnEquipmentRemoved;
    
    // 이 부분 수정 했습니다 - 노희강 -
    // 이 변수는 플레이어가 갖고 있는 장비의 스탯을 모두 합산해서 반환하는 메서드를 부릅니다.
    [SerializeField]
    private BonusStatSystem bonusStatSystem;

    // 이 변수는 플레이어 공격력, 공격 범위, 공격 속도(애니메이션 속도), 이동 속도, 추가 체력 등...
    // 스탯을 합산해놓기 때문에 바로 쓰실 수 있습니다.
    private BonusStat currentBonusStat;
    public BonusStat CurrentBonusStat => currentBonusStat;

    public void StoreEquipment(Vector3 pos, EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        EquipmentType equipmentType = equipmentData.EquipmentType;

        equipmentItems[equipmentType] = equipmentData;

        // 이 부분 수정 했습니다 - 노희강 -
        // 타입은 데이터 안에 존재하기 때문에 없애고
        // 위치값이 필요해서 position을 넣었습니다.
        OnEquipmentStored?.Invoke(pos, equipmentData);

        // 이 부분 수정 했습니다 - 노희강 -
        // bonusStatSystem 변수를 통해 모든 장비들의 스탯을 계산해서 반환해줍니다.
        // 그리고 BonusStat이라는 클래스는 합산된 결과값을 갖고 있습니다.
        currentBonusStat = bonusStatSystem.CalculateEquipmentStats(equipmentItems.Values.ToArray());

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