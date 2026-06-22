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
                EquipmentType equipmentTypeKey = (EquipmentType)Random.Range(0, 3);// Random.Range(0, 5);
                if(equipmentTypeKey == EquipmentType.Weapon)
                {
                    equipmentTypeKey = EquipmentType.Necklace;
                }
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
        // 인벤토리가 생겼다면 이렇게 함
        if (collision.TryGetComponent(out PlayerInventory player))
        {
            if (equipmentData != null)
            {
                player.StoreEquipment(transform.position, equipmentData);
            }

            gameObject.SetActive(false);
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