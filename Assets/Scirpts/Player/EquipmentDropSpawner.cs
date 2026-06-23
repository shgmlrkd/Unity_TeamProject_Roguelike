using UnityEngine;

public class EquipmentDropSpawner : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject equipmentItemPrefab;
    [SerializeField] private float droppedItemPickupDelay = 1.0f;

    private void Awake()
    {
        if (playerInventory == null)
        {
            playerInventory = GetComponent<PlayerInventory>();
        }
    }

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnEquipmentDropped += SpawnDroppedEquipment;
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnEquipmentDropped -= SpawnDroppedEquipment;
        }
    }

    private void SpawnDroppedEquipment(Vector3 position, EquipmentData equipmentData)
    {
        if (equipmentItemPrefab == null || equipmentData == null)
        {
            return;
        }

        GameObject itemObject = Instantiate(equipmentItemPrefab, position, Quaternion.identity);

        if (itemObject.TryGetComponent(out EquipmentItem equipmentItem))
        {
            equipmentItem.SetEquipmentData(equipmentData);
            equipmentItem.SetPickupDelay(droppedItemPickupDelay);
        }
    }
}