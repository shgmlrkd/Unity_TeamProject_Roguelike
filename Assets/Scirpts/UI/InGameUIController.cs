using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIController : MonoBehaviour
{
    [Header("스크린 페이더")]
    [SerializeField]
    private ScreenFader screenFader;

    [Header("옵션 애니메이션")]
    [SerializeField]
    private OptionUIAnimation optionUIAnimation;

    private bool isOptionOpen = false;

    private void Awake()
    {
        if (screenFader != null)
        {
            screenFader.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        screenFader.gameObject.SetActive(true);

        // 인게임 씬으로 넘어오면 페이드 인 진행
        screenFader.FadeIn(UIAnimationSettings.FadeDuration);
    }

    private void Update()
    {
        // ESC 버튼 누르면 옵션 패널 활성/비활성화
        OptionToggleInput();
    }

    private void OptionToggleInput()
    {
        // ESC를 누르면
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // 열려있으면
            if (isOptionOpen)
            {
                // 옵션 닫기
                CloseOption();
            }
            else // 아니면
            {
                // 옵션 열기
                OpenOption();
            }
        }
    }

    // 옵션 열고 게임 시간 멈추기
    private void OpenOption()
    {
        optionUIAnimation.Show();
        isOptionOpen = true;

        Time.timeScale = 0.0f;
    }

    // 옵션 닫고 게임 시간 재개
    private void CloseOption()
    {
        optionUIAnimation.Hide();
        isOptionOpen = false;

        Time.timeScale = 1.0f;
    }
}