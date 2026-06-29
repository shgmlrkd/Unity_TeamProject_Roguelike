using NUnit.Framework.Interfaces;
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

    // 풀에 아이템 넣기
    public void ReturnItem(Item item)
    {
        PoolManager.Instance.ReturnPool(item);
    }

    // 상자 아이템 드랍일 경우 사용
    public void DropItem(ItemData data, Vector3 pos)
    {
        SpawnItem(data, 0, 1, 0, pos);
    }

    // 여기서 아이템의 데이터를 랜덤으로 뽑고 적용함
    public void DropItem(int itemCount, int gold, Vector3 pos)
    {
        // 무조건 골드를 받아옴
        SpawnItem(randomItem.GetGoldItem(), 0, itemCount, gold, pos);

        // count가 2개 이상이면 장비, 소비, 꽝 중 랜덤으로 당첨됨
        for(int i = 1; i < itemCount; i++)
        {
            ItemData itemData = randomItem.GetRandomItem();

            // 꽝이면 넘어감
            if (itemData == null) continue; 

            SpawnItem(itemData, i, itemCount, 0, pos);
        }
    }

    // 풀에서 아이템 꺼내고 초기화
    private void SpawnItem(ItemData data, int index, int itemCount, int gold, Vector3 pos)
    {
        Item item = PoolManager.Instance.GetPool(itemPrefab);

        Vector3 itemPos = item.GetScatterOffset(index, itemCount);

        Vector3 finalPos = itemPos + pos;

        item.DropItemInit(data, gold, finalPos);
    }

    // 장착된 아이템을 드롭하는 메서드
    public Item DropEquippedItem(Vector3 pos, EquipmentData data)
    {
        Item item = PoolManager.Instance.GetPool(itemPrefab);

        item.OldEquipmentInit(data, pos);

        return item;
    }

    // 드랍된 아이템을 먹으면 장비창에 가는데
    // 만약 장비창에 이미 그 타입의 장비가 있다면 그 장비가 드랍되어있게 하는데
    // 이럴 경우 바닥에 있는 아이템은 예를 들어 30초 뒤에 사라진다 (비활성화) 이런 느낌으로 갈까요?
    // 이렇게 될 경우 그냥 그냥 드랍되고서부터 아이템 사라지는 시간이 코루틴으로 돌게될건데..
}