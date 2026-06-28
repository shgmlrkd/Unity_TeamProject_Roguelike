using DG.Tweening;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    private const float ZOOM_IN = 1.0f;
    private const float ZOOM_Out = 0.8f;

    [SerializeField]
    private Transform goldImageTransform;

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

        InGameManager.Instance.OnChangedGold += PlayGoldUIAnimation;
        InGameManager.Instance.OnChangedGold += UpdateGoldText;
    }

    private void OnDisable()
    {
        InGameManager.Instance.OnChangedGold -= PlayGoldUIAnimation;
        InGameManager.Instance.OnChangedGold -= UpdateGoldText;
    }

    private void UpdateGoldText(int gold)
    {
        goldText.text = $"x {gold.ToString()}";
    }

    private void PlayGoldUIAnimation(int gold)
    {
        Sequence sequence = DOTween.Sequence();

        goldImageTransform.DOKill();
        sequence.Append(goldImageTransform.DOScale(ZOOM_IN, UIAnimationSettings.NormalDuration).
                        OnComplete(() =>
                        {
                            goldImageTransform.DOScale(ZOOM_Out, UIAnimationSettings.NormalDuration);
                        }));
    }
}
