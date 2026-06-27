using System;
using UnityEngine;

public class InGameManager : ScenesSingleton<InGameManager>
{
    // 랜덤 스탯 굴리기 awake에서 하기 O
    // 스탯 플레이어에게 적용시키기 O
    // 플레이어 체력 여기서 계속 체크하기
    // 플레이어 체력 0 되면 GameOverUI에 알리는 이벤트 짜기 (action) 여기서 구독을 하면댐 플레이어가 죽었는가 또는 보스를 처치했는가
    // 몬스터 죽인 수 기록하기 (int) -
    // 보스 몬스터 처치 했는지 기록하기 (bool) -
    // 플레이 타임 기록하기 -
    // 획득한 골드 여기서 합산 또는 차감해서 goldtext에 알려주기 -

    [SerializeField]
    private StartStatRoller playerStat;

    [SerializeField]
    private PlayerHP playerhp;

    [SerializeField]
    private InGameTimer timerText;

    private int collectedGold = 0;      // 상점 또는 보물방에 상자를 열 때 쓸 골드

    private string gameOverTime = "";   // 게임 오버 시간
    private int monsterKillCount = 0;   // 몬스터 죽인 수
    private bool isKilledBoss = false;  // 보스 처치 했는가
    private bool isGameOver = false;

    public string GameOverTime => gameOverTime;

    public event Action<int> OnChangedGold;
    public event Action OnGameOver;

    public bool IsGameOver => isGameOver;

    protected override void Awake()
    {
        base.Awake();

        playerStat.RollStats();         // 플레이어 랜덤 스탯 뽑기
    }

    private void OnEnable()
    {
        isGameOver = false;
        playerhp.OnPlayerDead += RegisterGameOverTime;
        playerhp.OnPlayerDead += ShowGameOverUI;
    }

    private void OnDisable()
    {
        // 이건 나중에 수정
        // 이유 : 구독 해제 타이밍 안맞음
        playerhp.OnPlayerDead -= RegisterGameOverTime;
        playerhp.OnPlayerDead -= ShowGameOverUI;
    }

    private void ShowGameOverUI()
    {
        isGameOver = true;
        OnGameOver?.Invoke();
    }

    // 몬스터 한 마리 죽일 때마다 호출할 함수
    public void RegisterMonsterKill()
    {
        monsterKillCount++;
    }

    // 플레이어가 죽는 시간을 저장할 함수
    public void RegisterGameOverTime()
    {
        gameOverTime = $"플레이 타임 [ {timerText.TimerText} ]";
    }

    // 보스 처치 결과 텍스트
    public string GetBossKillResult()
    {
        string result = isKilledBoss ? "성공" : "실패";

        return $"보스 처치 [ {result} ]";
    }

    public string GetMonsterKillResult()
    {
        return $"몬스터 처치 [ {monsterKillCount} 마리 ]";
    }

    // 플레이어가 먹은 골드 수집 (나중에 상점 또는 보물방에서 상자 열 때 차감)
    public void CollectedGold(int gold)
    {
        collectedGold += gold;
        // 이벤트 써서 골드 텍스트 갱신
        OnChangedGold?.Invoke(collectedGold);
        print($"{collectedGold}");
    }

    // 플레이어가 골드를 사용할 때 호출
    public bool UseGold(int gold)
    {
        if (collectedGold < gold)
        {
            return false;
        }

        collectedGold -= gold;
        // 이벤트 써서 골드 텍스트 갱신
        OnChangedGold?.Invoke(collectedGold);
        return true;
    }

    // 보스 처치하면 이벤트로 함수 호출
    public void RegisterBossKill()
    {
        isKilledBoss = true;
    }
}