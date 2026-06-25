using UnityEngine;

public class PlayerStartStatApplier : MonoBehaviour
{
    //랜덤스탯적용
    [SerializeField] private PlayerHP playerHp;
    [SerializeField] private PlayerStartStatBonus startStatBonus;

    private void Awake()
    {
        if (playerHp == null)
        {
            playerHp = GetComponent<PlayerHP>();
        }

        if (startStatBonus == null)
        {
            startStatBonus = GetComponent<PlayerStartStatBonus>();
        }

        ApplyStartStat();
    }

    private void ApplyStartStat()
    {
        if (!PlayerStartStatStorage.HasRolledStat)
        {
            return;
        }

        PlayerRolledStat stat = PlayerStartStatStorage.RolledStat;

        if (playerHp != null)
        {
            playerHp.SetMaxHp(stat.MaxHp);
            playerHp.SetBonusHp(stat.ShieldHp);
        }

        if (startStatBonus != null)
        {
            startStatBonus.SetStartBonusStat(stat);
        }
    }
}