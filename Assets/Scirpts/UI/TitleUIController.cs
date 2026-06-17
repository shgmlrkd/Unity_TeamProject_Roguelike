using UnityEngine;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject optionPanel;

    public void OnClickGameStart()
    {
        print("게임 시작");
    }

    public void OnClickEnterOption()
    {
        // 옵션 패널 활성화
        optionPanel.SetActive(true);
    }

    public void OnClickExitOption()
    {
        // 여기서 GameManager에 볼륨과 표시 여부를 저장 시키면댐

        // 옵션 패널 비활성화
        optionPanel.SetActive(false);
    }

    public void OnClickExit()
    {
        print("게임 종료");
    }
}