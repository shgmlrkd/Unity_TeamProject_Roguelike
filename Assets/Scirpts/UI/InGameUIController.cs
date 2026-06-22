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

    [Header("장비 UI 패널")]
    [SerializeField]
    private GameObject equipmentUI;

    [Header("스탯 UI 패널")]
    [SerializeField]
    private GameObject statUI;

    private bool isOptionOpen = false;

    private void Start()
    {
        // 옵션 설정값을 기반으로 UI 활성/비활성 상태 갱신
        ApplyUIVisibility(OptionManager.Instance.ShowStat, OptionManager.Instance.ShowEquip);

        screenFader.gameObject.SetActive(true);

        // 인게임 씬으로 넘어오면 페이드 인 진행
        screenFader.FadeIn(UIAnimationSettings.FadeDuration);
    }

    private void OnEnable()
    {
        // 이벤트 등록
        OptionManager.Instance.OnUIOptionChanged += ApplyUIVisibility;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        OptionManager.Instance.OnUIOptionChanged -= ApplyUIVisibility;
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
    public void CloseOption()
    {
        optionUIAnimation.Hide();
        isOptionOpen = false;

        Time.timeScale = 1.0f;
    }

    // 옵션에서 설정한 부분만 표시 해주기 (장비, 스탯)
    private void ApplyUIVisibility(bool showStat, bool showEquip)
    {
        print($"InGameUIController : {OptionManager.Instance.ShowStat} / {OptionManager.Instance.ShowEquip}");

        statUI.SetActive(showStat);
        equipmentUI.SetActive(showEquip);
    }
}