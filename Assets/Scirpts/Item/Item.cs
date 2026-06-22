using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemDataBase itemDataBase;

    private EquipmentData equipmentData = null;
    private ConsumableData consumableData = null;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 이건 장비 타입인지 소비 타입인지 랜덤 돌린거
        int itemType = 0;// Random.Range(0, 2);

        switch ((ItemType)itemType)
        {
            // 장비다
            case ItemType.Equipment:
                EquipmentType equipmentTypeKey = (EquipmentType)2;// Random.Range(0, 5);
                equipmentData = itemDataBase.GetEquipmentData(equipmentTypeKey);
                break;

            // 소비다
            case ItemType.Consumable:
                ConsumableType consumableTypeKey = (ConsumableType)Random.Range(0, 1);
                consumableData = itemDataBase.GetConsumableData(consumableTypeKey);
                break;
        }

        ItemData itemdata = null;

        if (equipmentData != null) 
        {
            itemdata = equipmentData;
        }
        else if(consumableData != null)
        {
            itemdata = consumableData;
        }

        if(itemdata == null)
        {
            print("아이템 데이터가 없음");
            return;
        }

        spriteRenderer.sprite = itemdata.Sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerHP player))
        {
            if (equipmentData != null)
            {
                // 플레이어 담당자와 상의 해야함
                player.SetBonusHp(equipmentData.ShieldHp);
            }

            gameObject.SetActive(false);
        }
    }
}