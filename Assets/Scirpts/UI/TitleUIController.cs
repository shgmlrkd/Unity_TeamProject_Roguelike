using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [Header("옵션 애니메이션")]
    [SerializeField]
    private OptionUIAnimation optionUIAnimation;

    [Header("스크린 페이더")]
    [SerializeField] 
    private ScreenFader screenFader;

    [Header("타이틀 캔버스 그룹")]
    [SerializeField]
    private CanvasGroup titleCanvasGroup;

    public void OnClickGameStart()
    {
        Sequence sequence = DOTween.Sequence();

        // 페이드 아웃 진행
        sequence.Append(screenFader.FadeOut(UIAnimationSettings.FadeDuration));

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
        // 옵션 애니메이션 후 보여주기 (열기)
        optionUIAnimation.Show();
    }

    public void OnClickCloseOption()
    {
        // 옵션 애니메이션 후 숨기기 (닫기)
        optionUIAnimation.Hide();
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