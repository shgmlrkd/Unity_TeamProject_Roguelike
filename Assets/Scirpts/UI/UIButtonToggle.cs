using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonToggle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject checkObj;

    private bool isOn = true;
    public bool IsOn => isOn;

    public void OnPointerClick(PointerEventData eventData)
    {
        isOn = !isOn;

        checkObj.SetActive(isOn);
    }
}