using UnityEngine;
using DG.Tweening;

public class TitleMove : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);

    // 타이틀 애니메이션 InBounce
    private Vector3 targetScale = new Vector3(1.05f, 1.05f, 1.05f);
   
    private float outElasticDuration = 1.0f;
    private float inBounceDuration = 1.5f;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        StartTitleAnimation();
    }

    private void StartTitleAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        // OutElastic 딱 한번 실행 (고무처럼 늘어났다가 원상태로 복귀)
        sequence.Append(transform.DOScale(originalScale, outElasticDuration).From(startScale).SetEase(Ease.OutElastic));

        // OutElastic 끝나면 InBounce를 무한 반복
        sequence.AppendCallback(() =>
        {
            transform.DOScale(targetScale, inBounceDuration).SetEase(Ease.InBounce).SetLoops(-1, LoopType.Yoyo);
        });
    }
}