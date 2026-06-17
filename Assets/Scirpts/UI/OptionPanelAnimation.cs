using DG.Tweening;
using UnityEngine;

public class OptionPanelAnimation : MonoBehaviour
{
    [Header("애니메이션 실행 시간")]
    [SerializeField]
    private float duration = 0.2f;
    public float Duration => duration;
    
    /// <summary>
    /// 옵션 패널 열기 애니메이션
    /// 0 → 1 스케일로 확대
    /// </summary>
    public Tween PlayOpenOptionPanel()
    {
        return transform.DOScale(Vector3.one, duration).From(Vector3.zero).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 옵션 패널 닫기 애니메이션
    /// 1 → 0 스케일로 축소
    /// </summary>
    public Tween PlayCloseOptionPanel()
    {
        return transform.DOScale(Vector3.zero, duration).From(Vector3.one).SetEase(Ease.OutQuad);
    }
}