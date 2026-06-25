using UnityEngine;

public class PlayerStartStatBonus : MonoBehaviour
{
    private PlayerRolledStat startStat;

    public int Attack => startStat != null ? startStat.Attack : 0;
    public float AttackSpeedRate => startStat != null ? startStat.AttackSpeedRate : 0f;
    public float MoveSpeedRate => startStat != null ? startStat.MoveSpeedRate : 0f;
    public int ShieldHp => startStat != null ? startStat.ShieldHp : 0;

    public void SetStartBonusStat(PlayerRolledStat stat)
    {
        startStat = stat;
    }
}