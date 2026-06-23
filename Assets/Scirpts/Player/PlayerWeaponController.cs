using System;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponData defaultWeapon;
    [SerializeField] private WeaponData currentWeapon;

    private PlayerInventory playerInventory;

    public WeaponData CurrentWeapon => currentWeapon;

    public event Action<WeaponData> OnWeaponChanged;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();

        if (currentWeapon == null)
        {
            currentWeapon = defaultWeapon;
        }
    }
    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnEquipmentStored += HandleEquipmentStored;
            playerInventory.OnEquipmentRemoved += HandleEquipmentRemoved;
        }
    }
    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnEquipmentStored -= HandleEquipmentStored;
            playerInventory.OnEquipmentRemoved -= HandleEquipmentRemoved;
        }
    }
    private void Start()
    {
        // 기본 무기도 인벤토리에 저장해야 CurrentBonusStat.Attack에 포함됨
        if (playerInventory != null && currentWeapon != null)
        {
            if (!playerInventory.HasEquipment(currentWeapon.EquipmentType))
            {
                playerInventory.StoreEquipment(transform.position, currentWeapon);
            }
        }

        OnWeaponChanged?.Invoke(currentWeapon);
    }
    //들고 있던 무기 떨어트리고 장착
    // 기존 WeaponItem 쪽 코드 호환용
    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null)
        {
            return;
        }

        if (playerInventory != null)
        {
            playerInventory.StoreEquipment(transform.position, newWeapon);
            return;
        }

        // 인벤토리가 없을 때만 예외적으로 직접 장착
        currentWeapon = newWeapon;
        OnWeaponChanged?.Invoke(currentWeapon);
    }
    //무기 떨어트리는 위치 제어
    private void HandleEquipmentStored(Vector3 pos, EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            return;
        }

        if (equipmentData.EquipmentType != EquipmentType.Weapon)
        {
            return;
        }

        if (equipmentData is not WeaponData weaponData)
        {
            //Debug.LogError("Weapon 타입 장비인데 WeaponData가 아닙니다.");
            return;
        }

        currentWeapon = weaponData;

        OnWeaponChanged?.Invoke(currentWeapon);

        //Debug.Log($"무기 장착: {currentWeapon.WeaponName}");
    }
    private void HandleEquipmentRemoved(EquipmentType equipmentType)
    {
        if (equipmentType != EquipmentType.Weapon)
        {
            return;
        }

        currentWeapon = null;

        OnWeaponChanged?.Invoke(currentWeapon);
    }
}