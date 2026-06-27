using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] gameOverTexts;
 
    [SerializeField]
    private Button[] gameOverButtons;

    public void Result()
    {
        // 플레이 타임
        gameOverTexts[(int)GameOverTextType.PlayTime].text =
                                InGameManager.Instance.GameOverTime;
        
        // 보스 처치 유무
        gameOverTexts[(int)GameOverTextType.BossClearCheck].text =
                                InGameManager.Instance.GetBossKillResult();

        // 몬스터 처치 수
        gameOverTexts[(int)GameOverTextType.MonsterKillCount].text =
                                InGameManager.Instance.GetMonsterKillResult();
    }

    // 다시 하기
    public void OnClickRetry()
    {
        GameSceneManager.Instance.ReloadCurScene();
    }

    // 메인 메뉴
    public void OnClickMainMenu()
    {
        GameSceneManager.Instance.LoadScene(SceneType.Title);
    }

    // 게임 종료
    public void OnClickQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}