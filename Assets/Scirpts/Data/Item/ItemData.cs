using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "GameData/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private ItemType itemType;

    [SerializeField]
    private string itemName;

    [SerializeField]
    private Sprite sprite;

    public ItemType ItemType => itemType;   // 아이템 타입
    public string ItemName => itemName;     // 아이템 이름
    public Sprite Sprite => sprite;         // 아이템 이미지
}