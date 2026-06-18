using DG.Tweening;
using UnityEngine;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject optionPanel;

    [SerializeField]
    private OptionPanelAnimation uiAnimation;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float inputToggleRatio = 0.25f;

    public void OnClickGameStart()
    {
        print("게임 시작");
    }

    public void OnClickOpenOption()
    {
        // 옵션 패널 활성화
        optionPanel.SetActive(true);

        // 애니메이션 재생 중 입력 비활성화
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Sequence sequence = DOTween.Sequence();

        // 옵션 패널 열기 애니메이션 재생
        sequence.Append(uiAnimation.PlayOpenOptionPanel());

        // 애니메이션이 일정 비율 진행되면 입력 활성화
        sequence.InsertCallback(uiAnimation.Duration * inputToggleRatio, () =>
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        });
    }

    public void OnClickCloseOption()
    {
        // 옵션 설정 저장
        // 여기서 GameManager에 볼륨과 표시 여부를 저장 시키면댐

        Sequence sequence = DOTween.Sequence();

        // 옵션 패널 닫기 애니메이션 재생
        sequence.Append(uiAnimation.PlayCloseOptionPanel());

        // 애니메이션이 일정 비율 진행되면 입력 비활성화
        sequence.InsertCallback(uiAnimation.Duration * inputToggleRatio, () =>
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        });

        // 애니메이션 종료 후 패널 비활성화
        sequence.OnComplete(() =>
        {
            optionPanel.SetActive(false);
        });
    }

    public void OnClickExit()
    {
        print("게임 종료");
    }
}