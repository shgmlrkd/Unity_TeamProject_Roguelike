using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUIController : MonoBehaviour
{
    [SerializeField]
    private Image[] equipmentImages;

    [SerializeField]
    private PlayerInventory inventory;

    [SerializeField]
    private EquipmentImageManager equipmentImageManager;

    private Image[] prevImages = new Image[(int)EquipmentType.Length];

    private void OnEnable()
    {
        // 플레이어 인벤토리 구현 시 이벤트에 구독 
        inventory.OnEquipmentStored += UpdateEquipmentUI;   
    }

    private void OnDisable()
    {
        // 플레이어 인벤토리 구현 시 이벤트에 구독 해제
        inventory.OnEquipmentStored -= UpdateEquipmentUI;   
    }

    // 아이템 획득 시 장비 타입에 맞는 UI에 표시
    private void UpdateEquipmentUI(Vector3 pos, EquipmentData data)
    {
        // 이미지 설정 후 활성화
        Image image = equipmentImageManager.EquipmentImagePop();
        image.sprite = data.Sprite;

        // 이미지의 위치를 아이템의 위치로 바꾼 후 보간해서 이동 후 도착하면 활성화
        Transform imageTransform = equipmentImages[(int)data.EquipmentType].transform;

        // 처음 위치로 가야함 그게 타겟위치
        Vector2 targetPos = imageTransform.position;

        // 스크린좌표로 변환
        Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
        image.transform.position = screenPos;

        // 움직임
        image.transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if(prevImages[(int)data.EquipmentType] != null)
            {
                prevImages[(int)data.EquipmentType].gameObject.SetActive(false);
            }

            prevImages[(int)data.EquipmentType] = image;
        });
    }
}