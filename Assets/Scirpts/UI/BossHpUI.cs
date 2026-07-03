using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    [Header("Hp UI 총괄 오브젝트")]
    [SerializeField]
    private GameObject bossHpObj;

    [Header("HP 이미지")]
    [SerializeField]
    private Image hpImage;

    [Header("HP 연출 이미지")]
    [SerializeField]
    private Image damageImage;

    [SerializeField]
    private CanvasGroup canvasGroup;

    private BossHp bossHp;

    private const float FADE_DURATION = 1.5f;
    private const float FADE_DELAY_DURATION = 1.0f;

    private const float DAMAGED_LEADING_BAR_DURATION = 2.0f;

    private const float HEAL_LEADING_BAR_DURATION = 1.5f;
    private const float HEAL_TRAILING_BAR_DURATION = 3.5f;

    private void Awake()
    {
        bossHpObj.SetActive(false);
    }

    private void OnEnable()
    {
        BossSpawnManager.Instance.OnShowBossHpUI += ShowBossHpUI;
    }

    private void OnDisable()
    {
        if (bossHp != null)
        {
            bossHp.OnBossHit -= PlayDamageHpUI;
            bossHp.OnBossHeal -= PlayHealHpUI;
        }

        if (BossSpawnManager.Instance != null)
        {
            BossSpawnManager.Instance.OnShowBossHpUI -= ShowBossHpUI;
        }
    }

    private void Start()
    {
        bossHp = InGameManager.Instance.BossHp;

        bossHp.OnBossHit += PlayDamageHpUI;
        bossHp.OnBossHeal += PlayHealHpUI;
    }

    // 보스 체력바 연출
    private void PlayDamageHpUI(int curHp, int maxHp)
    {
        float target = (float)curHp / maxHp;

        hpImage.fillAmount = target;

        damageImage.DOKill();
        damageImage.DOFillAmount(target, DAMAGED_LEADING_BAR_DURATION).SetEase(Ease.OutCubic);
    }

    private void PlayHealHpUI(int curHp, int maxHp)
    {
        float target = (float)curHp / maxHp;

        Sequence seq = DOTween.Sequence();

        seq.Join(damageImage.DOFillAmount(target, HEAL_LEADING_BAR_DURATION));

        seq.Join(hpImage.DOFillAmount(target, HEAL_TRAILING_BAR_DURATION).SetEase(Ease.OutCubic));

        seq.OnComplete(() =>
        {
            bossHp.SetInvincible(false);
        });
    }

    // 보스 체력바 UI 활성화 후 연출 시작
    private void ShowBossHpUI()
    {
        bossHpObj.SetActive(true);

        PlayBossHpUIIntroCoroutine();
    }

    // 인트로 페이드 인 후 체력 차는 연출
    private void PlayBossHpUIIntroCoroutine()
    {
        canvasGroup.alpha = 0.0f;
        hpImage.fillAmount = 0.0f;
        damageImage.fillAmount = 0.0f;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(FADE_DELAY_DURATION);

        seq.Append(canvasGroup.DOFade(1.0f, FADE_DURATION));

        seq.Append(hpImage.DOFillAmount(1.0f, FADE_DURATION).SetEase(Ease.OutCubic));

        seq.AppendCallback(() =>
        {
            damageImage.fillAmount = 1.0f;
        });
    }
}