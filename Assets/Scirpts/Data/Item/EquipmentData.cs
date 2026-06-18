using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "GameData/EquipmentData")]
public class EquipmentData : ItemData
{
    [SerializeField]
    private EquipmentType equipmentType;

    [SerializeField]
    private int attack;

    [SerializeField]
    private float attackRange;

    [SerializeField]
    private float attackSpeedRate;

    [SerializeField]
    private float moveSpeedRate;

    public EquipmentType EquipmentType => equipmentType; // 장비 타입
    public int Attack => attack;                        // 공격력
    public float AttackRange => attackRange;            // 공격 범위
    public float AttackSpeedRate => attackSpeedRate;    // 공격 속도 계수 (애니메이션 용도)
    public float MoveSpeedRate => moveSpeedRate;        // 이동 속도 계수
}