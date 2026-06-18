using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 originalScale;

    [Header("마우스 오버 크기")]
    [SerializeField]
    private float hoverScale = 1.1f;

    [Header("튕김 강도")]
    [SerializeField]
    private float punchPower = 0.1f;

    [Header("시간")]
    [SerializeField]
    private float duration = 0.2f;

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
        // usePunch가 false면 클릭 시 애니메이션 적용 X
        if (!usePunch) return;

        // 클릭 시 애니메이션 효과를 적용함
        transform.DOKill();
        transform.DOPunchScale(originalScale * punchPower, duration);
    }

    // 마우스 포인터가 버튼에 오버랩 될 때 호출되는 인터페이스 메서드
    public void OnPointerEnter(PointerEventData eventData)
    {
        // useHover가 false면 마우스가 오버랩 시 애니메이션 적용 X
        if (!useHover) return;

        // 오버랩 시 애니메이션 효과를 적용함
        transform.DOKill();
        transform.DOScale(Vector3.one * hoverScale, duration).SetEase(Ease.OutQuad);
    }

    // 마우스 포인터가 버튼에 오버랩되어 있다가 빠져나올 때 호출되는 인터페이스 메서드
    public void OnPointerExit(PointerEventData eventData)
    {
        // useHover가 false면 아래 코드는 의미가 없으므로 애니메이션 적용 X
        if (!useHover) return;

        // 마우스가 벗어날 때 애니메이션 효과를 적용함
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }
}