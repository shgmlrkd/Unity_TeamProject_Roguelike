using UnityEngine;
using UnityEngine.EventSystems;

public class UISliderSound : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX(SoundKey.ButtonHover);
    }
}
