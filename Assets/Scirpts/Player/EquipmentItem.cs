using UnityEngine;

public class PlayerEquipmentItem : MonoBehaviour
{
    [SerializeField] private EquipmentData equipmentData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerInventory playerInventory))
        {
            return;
        }

        playerInventory.StoreEquipment(transform.position, equipmentData);

        Destroy(gameObject);
    }
}