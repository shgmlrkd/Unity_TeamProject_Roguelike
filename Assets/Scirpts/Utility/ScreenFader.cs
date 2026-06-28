using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == SceneNames.GetSceneName(SceneType.Title))
        { 
            FadeIn(UIAnimationSettings.FadeDuration);
        }
    }

    // 페이드 인
    public Tween FadeOut(float duration)
    {
        return fadeImage.DOFade(1.0f, duration).From(0.0f);
    }

    // 페이드 아웃
    public Tween FadeIn(float duration)
    {
        return fadeImage.DOFade(0.0f, duration).From(1.0f);
    }
}