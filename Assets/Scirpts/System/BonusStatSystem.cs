using UnityEngine;

public class BonusStat
{
    public int Attack = 0;
    public float AttackRange = 0.0f;
    public float AttackSpeedRate = 0.0f;
    public float MoveSpeedRate = 0.0f;
    public int ShieldHp = 0;
}

public class BonusStatSystem : MonoBehaviour
{
    // 장비 장착된 스탯을 계산해서 반환
    public BonusStat CalculateEquipmentStats(EquipmentData[] equipments)
    {
        BonusStat stat = new BonusStat();

        foreach (EquipmentData equipment in equipments)
        {
            if (equipment == null) continue;

            stat.Attack += equipment.Attack;
            stat.AttackRange += equipment.AttackRange;
            stat.AttackSpeedRate += equipment.AttackSpeedRate;
            stat.MoveSpeedRate += equipment.MoveSpeedRate;
            stat.ShieldHp += equipment.ShieldHp;
        }

        return stat;
    }
}