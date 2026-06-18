using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [Header("옵션 패널")]
    [SerializeField]
    private GameObject optionPanel;

    [Header("페이드 아웃 패널")]
    [SerializeField]
    private Image fadeOutPanel;

    [Header("타이틀 캔버스 그룹")]
    [SerializeField]
    private CanvasGroup titleCanvasGroup;
    
    [Header("옵션 캔버스 그룹")]
    [SerializeField]
    private CanvasGroup optionCanvasGroup;

    private void Awake()
    {
        if(fadeOutPanel != null)
        {
            fadeOutPanel.gameObject.SetActive(false);
        }
    }

    public void OnClickGameStart()
    {
        fadeOutPanel.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
       
        sequence.Append(fadeOutPanel.DOFade(1.0f, UIAnimationSettings.FadeDuration).From(0.0f));

        // 페이드 인이 일정 비율 진행되면 입력 비활성화
        sequence.InsertCallback(UIAnimationSettings.CallbackRatio, () =>
        {
            titleCanvasGroup.blocksRaycasts = false;
        });

        // 끝나면 DOTween 종료 시키고 인게임 씬 로드
        sequence.OnComplete(() =>
        {
            DOTween.KillAll();
            GameSceneManager.Instance.LoadScene(SceneType.InGame);
        });

        print("게임 시작");
    }

    public void OnClickOpenOption()
    {
        // 옵션 패널 활성화
        optionPanel.SetActive(true);

        // 애니메이션 재생 중 입력 비활성화
        optionCanvasGroup.blocksRaycasts = false;

        Sequence sequence = DOTween.Sequence();

        // 옵션 패널 열기 애니메이션 재생
        sequence.Append(UIAnimationUtility.ShowScale(optionPanel.transform, 0.2f));

        // 애니메이션이 일정 비율 진행되면 입력 활성화
        sequence.InsertCallback(UIAnimationSettings.CallbackRatio, () =>
        {
            optionCanvasGroup.blocksRaycasts = true;
        });
    }

    public void OnClickCloseOption()
    {
        Sequence sequence = DOTween.Sequence();

        // 옵션 패널 닫기 애니메이션 재생
        sequence.Append(UIAnimationUtility.HideScale(optionPanel.transform, 
            UIAnimationSettings.NormalDuration));

        // 애니메이션이 일정 비율 진행되면 입력 비활성화
        sequence.InsertCallback(UIAnimationSettings.CallbackRatio, () =>
        {
            optionCanvasGroup.blocksRaycasts = false;
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

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}