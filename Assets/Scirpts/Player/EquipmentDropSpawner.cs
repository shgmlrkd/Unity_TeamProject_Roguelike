using UnityEngine;

public class EquipmentDropSpawner : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
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
        // 장착했었던 장비 데이터가 없을 때
        if(equipmentData == null)
        {
            return;
        }

        // 이걸 통해 아이템이 position에 장착되있었던 아이템 데이터로 Drop함
        Item equippedItem = ItemManager.Instance.DropEquippedItem(position, equipmentData);
        equippedItem.SetPickupDelay(droppedItemPickupDelay);
    }
}