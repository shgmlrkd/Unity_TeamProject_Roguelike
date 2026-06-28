using DG.Tweening;
using UnityEngine;

public static class UIAnimationUtility
{
    // UI를 0 -> 1 스케일로 확대하며 표시하는 애니메이션
    public static Tween ShowScale(Transform target, float duration, Ease ease = Ease.OutQuad)
    {
        return target.DOScale(Vector3.one, duration).From(Vector3.zero).SetEase(ease);
    }

    // UI를 1 -> 0 스케일로 축소하며 표시하는 애니메이션
    public static Tween HideScale(Transform target, float duration, Ease ease = Ease.OutQuad)
    {
        return target.DOScale(Vector3.zero, duration).From(Vector3.one).SetEase(ease);
    }

    // 마우스 오버랩 시 지정한 크기로 확대하는 애니메이션
    public static Tween HoverScale(Transform target, float scale, float duration, Ease ease = Ease.OutQuad)
    {
        return target.DOScale(Vector3.one * scale, duration).SetEase(ease);
    }

    // 원래 크기로 복귀하는 애니메이션
    public static Tween ReturnScale(Transform target, Vector3 originalScale, float duration, Ease ease = Ease.OutQuad)
    {
        return target.DOScale(originalScale, duration).SetEase(ease);
    }

    // 클릭 시 튕기는 스케일 애니메이션
    public static Tween PunchScale(Transform target, float power, float duration)
    {
        return target.DOPunchScale(Vector3.one * power, duration);
    }
}