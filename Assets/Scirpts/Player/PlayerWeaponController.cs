using System;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponData defaultWeapon;
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private float weaponDropDistance = 0.8f;
    [SerializeField] private float droppedWeaponPickupDelay = 2.0f;

    private CharacterController2D characterController;

    public WeaponData CurrentWeapon => currentWeapon;

    public event Action<WeaponData> OnWeaponChanged;

    private void Awake()
    {
        characterController = GetComponent<CharacterController2D>();

        if (currentWeapon == null)
        {
            currentWeapon = defaultWeapon;
        }
    }

    private void Start()
    {
        OnWeaponChanged?.Invoke(currentWeapon);
    }
    //들고 있던 무기 떨어트리고 장착
    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null)
        {
            return;
        }

        DropCurrentWeapon();

        currentWeapon = newWeapon;
        OnWeaponChanged?.Invoke(currentWeapon);

        Debug.Log($"무기 장착: {currentWeapon.WeaponName}");
    }
    //무기 떨어트리는 위치 제어
    private void DropCurrentWeapon()
    {
        if (currentWeapon == null || currentWeapon.WeaponItemPrefab == null)
        {
            return;
        }

        Vector2 lookDirection = Vector2.down;

        if (characterController != null)
        {
            lookDirection = characterController.LookDirection;
        }

        Vector2 dropDirection = -lookDirection;

        if (dropDirection == Vector2.zero)
        {
            dropDirection = Vector2.down;
        }

        Vector3 dropPosition = transform.position + (Vector3)(dropDirection.normalized * weaponDropDistance);

        GameObject droppedWeapon = Instantiate(
            currentWeapon.WeaponItemPrefab,
            dropPosition,
            Quaternion.identity
        );

        if (droppedWeapon.TryGetComponent(out WeaponItem weaponItem))
        {
            weaponItem.SetPickupDelay(droppedWeaponPickupDelay);
        }
    }
}