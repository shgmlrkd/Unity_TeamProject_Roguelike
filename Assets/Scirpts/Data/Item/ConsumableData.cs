using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableData", menuName = "GameData/ConsumableData")]
public class ConsumableData : ItemData
{
    [SerializeField]
    private ConsumableType consumableType;

    [SerializeField]
    private int healAmount;

    public ConsumableType ConsumableType => consumableType; // 소비
    public int HealAmount => healAmount;                    // 치료량
}