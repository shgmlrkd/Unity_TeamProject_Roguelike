using TMPro;
using UnityEngine;

public class InGameTimer : MonoBehaviour
{
    private const int SECONDS_PER_MINUTE = 60;

    [SerializeField]
    private TextMeshProUGUI timerText;

    private float playTimer;
    private bool isGameEnd = false;

    public string TimerText => timerText.text;

    private void OnEnable()
    {
        InGameManager.Instance.OnGameOver += SetTimerStop;
    }

    private void OnDisable()
    {   
        InGameManager.Instance.OnGameOver -= SetTimerStop;
    }

    private void Update()
    {
        if (isGameEnd) return;

        playTimer += Time.deltaTime;

        int minutes = (int)(playTimer / SECONDS_PER_MINUTE);
        int seconds = (int)(playTimer % SECONDS_PER_MINUTE);

        timerText.text = $"{minutes:00} : {seconds:00}";
    }

    public void SetTimerStop()
    {
        isGameEnd = true;
    }
}