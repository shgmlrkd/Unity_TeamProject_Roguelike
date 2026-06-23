using UnityEngine;

public class ItemManager : ScenesSingleton<ItemManager>
{
    [Header("아이템 프리팹")]
    [SerializeField]
    private Item itemPrefab;

    [Header("아이템 데이터베이스")]
    [SerializeField]
    private ItemDataBase itemDataBase;

    private int poolSize = 30;

    private RandomItemSystem randomItem;

    protected override void Awake()
    {
        base.Awake();
        PoolManager.Instance.SetCreatePool();
        PoolManager.Instance.PreloadPool(itemPrefab, poolSize);

        randomItem = new RandomItemSystem(itemDataBase);
    }

    public void ReturnItem(Item item)
    {
        PoolManager.Instance.ReturnPool(item);
    }

    // 여기서 아이템의 데이터를 랜덤으로 뽑고 적용함
    public void DropItem(Vector3 pos)
    {
        ItemData itemData = randomItem.GetRandomItem();
        
        Item item = PoolManager.Instance.GetPool(itemPrefab);

        item.Initialize(itemData, pos);
    }

    // 장착된 아이템을 드롭하는 메서드
    public Item DropEquippedItem(Vector3 pos, EquipmentData data)
    {
        Item item = PoolManager.Instance.GetPool(itemPrefab);

        item.Initialize(data, pos);

        return item;
    }

    // 드랍된 아이템을 먹으면 장비창에 가는데
    // 만약 장비창에 이미 그 타입의 장비가 있다면 그 장비가 드랍되어있게 하는데
    // 이럴 경우 바닥에 있는 아이템은 예를 들어 30초 뒤에 사라진다 (비활성화) 이런 느낌으로 갈까요?
    // 이렇게 될 경우 그냥 그냥 드랍되고서부터 아이템 사라지는 시간이 코루틴으로 돌게될건데..
}