using NUnit.Framework.Interfaces;
using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemData itemData = null;

    private EquipmentData equipmentData = null;
    private ConsumableData consumableData = null;

    private SpriteRenderer spriteRenderer;

    private float canPickupTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(ItemData data, Vector2 pos)
    {
        itemData = data;

        equipmentData = null;
        consumableData = null;

        if (data is EquipmentData equipment)
        {
            equipmentData = equipment;
        }
        else if (data is ConsumableData consumable)
        {
            consumableData = consumable;
        }

        UpdateVisual();
        transform.position = pos;
    }

    private void UpdateVisual()
    {
        if (equipmentData == null || spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = equipmentData.Sprite;
    }

    public void SetPickupDelay(float delay)
    {
        canPickupTime = Time.time + delay;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 아이템 줍기 쿨타임
        if (Time.time < canPickupTime)
        {
            return;
        }

        // 인벤토리가 생겼다면 이렇게 함
        if (collision.TryGetComponent(out PlayerInventory player))
        {
            if (equipmentData != null)
            {
                player.StoreEquipment(transform.position, equipmentData);
            }

            ItemManager.Instance.ReturnItem(this);
        }

        /* 하트 UI 표시 테스트용
        if (collision.TryGetComponent(out PlayerHP playe))
        {
            if (equipmentData != null)
            {
                playe.SetBonusHp(equipmentData.ShieldHp);
            }

            gameObject.SetActive(false);
        }*/
    }
}