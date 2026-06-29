using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonUIAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 originalScale;

    [SerializeField] 
    private bool usePunch = false;

    [SerializeField]
    private bool useHover = true;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    // 클릭 시 호출되는 인터페이스 메서드
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX(SoundKey.ButtonClick);

        // usePunch가 false면 클릭 시 애니메이션 적용 X
        if (!usePunch) return;

        // 클릭 시 애니메이션 효과를 적용함
        transform.DOKill();
        transform.DOPunchScale(originalScale * UIAnimationSettings.PunchPower, 
            UIAnimationSettings.NormalDuration);
    }

    // 마우스 포인터가 버튼에 오버랩 될 때 호출되는 인터페이스 메서드
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX(SoundKey.ButtonHover);

        // useHover가 false면 마우스가 오버랩 시 애니메이션 적용 X
        if (!useHover) return;

        // 오버랩 시 애니메이션 효과를 적용함
        transform.DOKill();
        transform.DOScale(Vector3.one * UIAnimationSettings.HoverScale, 
            UIAnimationSettings.NormalDuration).SetEase(Ease.OutQuad);
    }

    // 마우스 포인터가 버튼에 오버랩되어 있다가 빠져나올 때 호출되는 인터페이스 메서드
    public void OnPointerExit(PointerEventData eventData)
    {
        // useHover가 false면 아래 코드는 의미가 없으므로 애니메이션 적용 X
        if (!useHover) return;

        // 마우스가 벗어날 때 애니메이션 효과를 적용함
        transform.DOKill();
        transform.DOScale(originalScale, 
            UIAnimationSettings.NormalDuration).SetEase(Ease.OutQuad);
    }
}