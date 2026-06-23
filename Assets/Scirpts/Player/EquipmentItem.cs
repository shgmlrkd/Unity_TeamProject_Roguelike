using UnityEngine;

public class EquipmentItem : MonoBehaviour
{
    [SerializeField] private EquipmentData equipmentData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float canPickupTime;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        UpdateVisual();
    }

    public void SetEquipmentData(EquipmentData equipmentData)
    {
        this.equipmentData = equipmentData;
        UpdateVisual();
    }

    public void SetPickupDelay(float delay)
    {
        canPickupTime = Time.time + delay;
    }

    private void UpdateVisual()
    {
        if (equipmentData == null || spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = equipmentData.Sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < canPickupTime)
        {
            return;
        }

        if (equipmentData == null)
        {
            return;
        }

        if (!other.TryGetComponent(out PlayerInventory playerInventory))
        {
            return;
        }

        playerInventory.StoreEquipment(transform.position, equipmentData);

        Destroy(gameObject);
    }
}