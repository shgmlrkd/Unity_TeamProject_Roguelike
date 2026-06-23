using TMPro;
using UnityEngine;

public class InGameTimer : MonoBehaviour
{
    private const int SECONDS_PER_MINUTE = 60;

    [SerializeField]
    private TextMeshProUGUI timerText;

    private float playTimer;

    private void Update()
    {
        playTimer += Time.deltaTime;

        int minutes = (int)(playTimer / SECONDS_PER_MINUTE);
        int seconds = (int)(playTimer % SECONDS_PER_MINUTE);

        timerText.text = $"{minutes:00} : {seconds:00}";
    }
}