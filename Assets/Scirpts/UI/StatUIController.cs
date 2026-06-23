using TMPro;
using UnityEngine;

public class StatUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] statUITexts;

    [SerializeField]
    private PlayerInventory inventory;

    private void OnEnable()
    {
        inventory.OnBonusStatChanged += UpdateBonusStatUI;
    }

    private void OnDisable()
    {
        inventory.OnBonusStatChanged -= UpdateBonusStatUI;
    }

    private void UpdateBonusStatUI(BonusStat bonusStat)
    {
        if (statUITexts.Length == 0) return;

        statUITexts[(int)BonusType.MoveSpeed].text = $"{bonusStat.MoveSpeedRate}";
        statUITexts[(int)BonusType.Attack].text = $"{bonusStat.Attack}";
        statUITexts[(int)BonusType.AttackSpeed].text = $"{bonusStat.AttackSpeedRate}";
    }
}
