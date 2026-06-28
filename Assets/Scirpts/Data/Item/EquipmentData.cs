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

    [SerializeField]
    private int shieldHp;

    public EquipmentType EquipmentType => equipmentType;    // 장비 타입
    public int Attack => attack;                            // 공격력
    public float AttackRange => attackRange;                // 공격 범위
    public float AttackSpeedRate => attackSpeedRate;        // 공격 속도 계수 (애니메이션 용도)
    public float MoveSpeedRate => moveSpeedRate;            // 이동 속도 계수
    public int ShieldHp => shieldHp;                        // 추가 체력

    // 런타임 전용 복사본 생성 (원본 에셋 건드리지 않음)
    public EquipmentData CreateCopy()
    {
        EquipmentData copy = CreateInstance<EquipmentData>();
        copy.equipmentType = equipmentType;
        copy.attack = attack;
        copy.attackRange = attackRange;
        copy.attackSpeedRate =attackSpeedRate;
        copy.moveSpeedRate = moveSpeedRate;
        copy.shieldHp = shieldHp;
        return copy;
    }

    // 복사본에서만 호출할 setter
    public void SetShieldHp(int hp) => shieldHp = hp;
}