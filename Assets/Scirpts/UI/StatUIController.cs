using System;
using TMPro;
using UnityEngine;

public class StatUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] statUITexts;

    [SerializeField]
    private PlayerInventory inventory;

    [SerializeField]
    private UIButtonToggle statToggle;

    private void OnEnable()
    {
        // 인벤토리에서 아이템 주우면 스탯 변경 이벤트 구독
        inventory.OnBonusStatChanged += UpdateBonusStatUI;

        // 옵션 껐다 킬때 스탯 변경 이벤트 구독
        statToggle.OnToggleChanged += OnToggleChanged;
    }

    private void OnDisable()
    {
        // 인벤토리에서 아이템 주우면 스탯 변경 이벤트 구독 해제
        inventory.OnBonusStatChanged -= UpdateBonusStatUI;

        // 옵션 껐다 킬때 스탯 변경 이벤트 구독 해제
        statToggle.OnToggleChanged -= OnToggleChanged;
    }

    private void UpdateBonusStatUI(BonusStat bonusStat)
    {
        if (statUITexts.Length == 0) return;

        statUITexts[(int)BonusType.MoveSpeed].text = $"{bonusStat.MoveSpeedRate}";
        statUITexts[(int)BonusType.Attack].text = $"{bonusStat.Attack}";
        statUITexts[(int)BonusType.AttackSpeed].text = $"{bonusStat.AttackSpeedRate}";
    }

    private void OnToggleChanged(bool isOn)
    {
        UpdateBonusStatUI(inventory.CurrentBonusStat);
    }
}