using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    // 1. 플레이 타임
    // 2. 보스 처치 유무
    // 3. 몬스터 처치 수

    [SerializeField]
    private TextMeshProUGUI[] gameOverTexts;

    // 내일 이어서 버튼 구현 하기
    [SerializeField]
    private Button[] gameOverButtons;

    public void Result()
    {
        gameOverTexts[(int)GameOverTextType.PlayTime].text =
                                InGameManager.Instance.GameOverTime;
        
        gameOverTexts[(int)GameOverTextType.BossClearCheck].text =
                                InGameManager.Instance.GetBossKillResult();

        gameOverTexts[(int)GameOverTextType.MonsterKillCount].text =
                                InGameManager.Instance.GetMonsterKillResult();
    }
}