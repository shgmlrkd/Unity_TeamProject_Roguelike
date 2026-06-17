using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    //드랍 이후 다시 줍지못하는 대기 시간
    private float canPickupTime;

    public WeaponData WeaponData => weaponData;

    private void Awake()
    {
        canPickupTime = Time.time;
    }

    public void SetPickupDelay(float delay)
    {
        canPickupTime = Time.time + delay;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < canPickupTime)
        {
            return;
        }

        if (!other.TryGetComponent(out CharacterController2D characterController))
        {
            return;
        }

        characterController.EquipWeapon(weaponData);
        Destroy(gameObject);
    }
}