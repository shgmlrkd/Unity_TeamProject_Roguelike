using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<EquipmentType, EquipmentData> equipmentItems =
        new Dictionary<EquipmentType, EquipmentData>();

    // EquipmentType 대신 Vector3 (위치)
    public event Action<Vector3, EquipmentData> OnEquipmentStored;
    public event Action<EquipmentType> OnEquipmentRemoved;

    // 탈착죄너땅에 떨어질 장비를 알리는 이벤트
    public event Action<Vector3, EquipmentData> OnEquipmentDropped;

    //타 스크립트에서 스탯 변경 시점에 반응할 때 사용
    public event Action<BonusStat> OnBonusStatChanged;

    // 이 변수는 플레이어가 갖고 있는 장비의 스탯을 모두 합산해서 반환하는 메서드를 부릅니다.
    [SerializeField]
    private BonusStatSystem bonusStatSystem;

    // 이 변수는 플레이어 공격력, 공격 범위, 공격 속도(애니메이션 속도), 이동 속도, 추가 체력 등...
    // 스탯을 합산해놓기 때문에 바로 쓰실 수 있습니다.
    private BonusStat currentBonusStat = new BonusStat();
    public BonusStat CurrentBonusStat => currentBonusStat;

    private PlayerHP playerHp;
    private EquipmentData shieldCopyCache; 
    private int currentShieldHp = 0; // 현재 남은 방패 내구도 추적

    private void Awake()
    {
        if (bonusStatSystem == null)
        {
            bonusStatSystem = GetComponent<BonusStatSystem>();
        }

        playerHp = GetComponent<PlayerHP>();

        RecalculateBonusStat();
    }

    public bool StoreEquipment(Vector3 pos, EquipmentData equipmentData)
    {
        if (equipmentData == null)
            return false;

        EquipmentType equipmentType = equipmentData.EquipmentType;

        // 같은 타입 장비가 이미 있으면 기존 장비를 드롭 대상으로 보냄
        if (equipmentItems.TryGetValue(equipmentType, out EquipmentData oldEquipment))
        {
            // 장비 타입이 쉴드일 경우 비교를 통해 방패를 떨어뜨릴지 판단하기
            if (oldEquipment.EquipmentType == EquipmentType.Shield)
            {
                if (oldEquipment.ShieldHp >= equipmentData.ShieldHp)
                {
                    return false; // 실패
                }

                // 새 방패로 교체될 때 캐시 무효화
                if (shieldCopyCache != null)
                {
                    Destroy(shieldCopyCache);
                    shieldCopyCache = null;
                }
            }

            OnEquipmentDropped?.Invoke(pos, oldEquipment);
        }

        // 새 방패 장착 시 내구도 초기화
        if (equipmentType == EquipmentType.Shield)
        {
            currentShieldHp = equipmentData.ShieldHp;
        }

        equipmentItems[equipmentType] = equipmentData;

        // 타입은 데이터 안에 존재하기 때문에 없애고
        // 위치값이 필요해서 position을 넣었습니다.
        OnEquipmentStored?.Invoke(pos, equipmentData);

        // bonusStatSystem 변수를 통해 모든 장비들의 스탯을 계산해서 반환해줍니다.
        // 그리고 BonusStat이라는 클래스는 합산된 결과값을 갖고 있습니다.
        RecalculateBonusStat(equipmentType);

        return true; // 성공
    }

    /*public void StoreEquipment(Vector3 pos, EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        EquipmentType equipmentType = equipmentData.EquipmentType;

        // 같은 타입 장비가 이미 있으면 기존 장비를 드롭 대상으로 보냄
        if (equipmentItems.TryGetValue(equipmentType, out EquipmentData oldEquipment))
        {
            isShieldChangeable = true;

            // 장비 타입이 쉴드일 경우 비교를 통해 방패를 떨어뜨릴지 판단하기
            if (oldEquipment.EquipmentType == EquipmentType.Shield)
            {
                if (oldEquipment.ShieldHp >= equipmentData.ShieldHp)
                {
                    isShieldChangeable = false;
                }
            }

            // 쉴드가 변경이 불가능한가? 그렇다면 함수 종료
            if (!isShieldChangeable) return;
            
            OnEquipmentDropped?.Invoke(pos, oldEquipment); 
        }

        equipmentItems[equipmentType] = equipmentData;

        // 타입은 데이터 안에 존재하기 때문에 없애고
        // 위치값이 필요해서 position을 넣었습니다.
        
        OnEquipmentStored?.Invoke(pos, equipmentData);
        
        // bonusStatSystem 변수를 통해 모든 장비들의 스탯을 계산해서 반환해줍니다.
        // 그리고 BonusStat이라는 클래스는 합산된 결과값을 갖고 있습니다.
        RecalculateBonusStat();

        //Debug.Log($"장비 저장: {equipmentType} / {equipmentData.ItemName}");
    }*/

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

        RecalculateBonusStat(equipmentType);

        //Debug.Log($"장비 제거: {equipmentType}");
    }

    // 장비를 장착해제하며 땅에 떨어뜨릴 때 사용
    public bool UnequipEquipment(EquipmentType equipmentType, Vector3 dropPosition)
    {
        if (!equipmentItems.TryGetValue(equipmentType, out EquipmentData removedEquipment))
        {
            return false;
        }

        equipmentItems.Remove(equipmentType);

        OnEquipmentRemoved?.Invoke(equipmentType);
        OnEquipmentDropped?.Invoke(dropPosition, removedEquipment);

        RecalculateBonusStat(equipmentType);

        return true;
    }

    public void Clear()
    {
        equipmentItems.Clear();

        RecalculateBonusStat();
    }

    // 데미지 받을 때 외부에서 호출
    public void DamageShield(int damage)
    {
        currentShieldHp = Mathf.Max(0, currentShieldHp - damage);
        RecalculateBonusStat();
    }

    private void RecalculateBonusStat(EquipmentType equipmentType = EquipmentType.Length)
    {
        if (bonusStatSystem == null)
        {
            //Debug.LogError("PlayerInventory: BonusStatSystem이 연결되지 않았습니다.");
            currentBonusStat = new BonusStat();
            OnBonusStatChanged?.Invoke(currentBonusStat);
            return;
        }

        EquipmentData[] equipments = equipmentItems.Values.ToArray();

        for (int i = 0; i < equipments.Length; i++)
        {
            if (equipments[i].EquipmentType == EquipmentType.Shield)
            {
                if (equipments[i] == null) continue;

                if (shieldCopyCache == null)
                {
                    shieldCopyCache = equipments[i].CreateCopy();
                }

                shieldCopyCache.SetShieldHp(currentShieldHp);
                equipments[i] = shieldCopyCache;
                break;
            }
        }

        currentBonusStat = bonusStatSystem.CalculateEquipmentStats(equipments);

        OnBonusStatChanged?.Invoke(currentBonusStat);
    }
}