using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;

    private void Awake()
    {
        if(goldText == null)
        {
            goldText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        UpdateGoldText(0);
        InGameManager.Instance.OnChangedGold += UpdateGoldText;
    }

    private void OnDisable()
    {
        InGameManager.Instance.OnChangedGold -= UpdateGoldText;
    }

    private void UpdateGoldText(int gold)
    {
        goldText.text = $"x {gold.ToString()}";
    }
}
