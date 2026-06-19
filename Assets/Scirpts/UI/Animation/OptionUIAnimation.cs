using DG.Tweening;
using UnityEngine;

public class OptionUIAnimation : MonoBehaviour
{
    [Header("옵션 패널")]
    [SerializeField] 
    private Transform target;

    [Header("옵션 BG")]
    [SerializeField]
    private Transform targetBG;

    [Header("옵션 캔버스 그룹")]
    [SerializeField] 
    private CanvasGroup canvasGroup;

    public void Show()
    {
        // 옵션 패널 활성화
        target.gameObject.SetActive(true);

        // 애니메이션 재생 중 입력 비활성화
        canvasGroup.blocksRaycasts = false;

        Sequence sequence = DOTween.Sequence();

        // 옵션 패널 열기 애니메이션 재생
        sequence.Append(UIAnimationUtility.ShowScale(targetBG,
            UIAnimationSettings.NormalDuration)).SetUpdate(true);

        // 애니메이션이 일정 비율 진행되면 입력 활성화
        sequence.InsertCallback(UIAnimationSettings.CallbackRatio, () =>
        {
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void Hide()
    {
        Sequence sequence = DOTween.Sequence();

        // 옵션 패널 닫기 애니메이션 재생
        sequence.Append(UIAnimationUtility.HideScale(targetBG,
            UIAnimationSettings.NormalDuration)).SetUpdate(true);

        // 애니메이션이 일정 비율 진행되면 입력 비활성화
        sequence.InsertCallback(UIAnimationSettings.CallbackRatio, () =>
        {
            canvasGroup.blocksRaycasts = false;
        });

        // 애니메이션 종료 후 패널 비활성화
        sequence.OnComplete(() =>
        {
            target.gameObject.SetActive(false);
        });
    }
}
