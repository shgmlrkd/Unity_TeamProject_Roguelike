using DG.Tweening;
using System;
using System.Collections;
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

    private Image[] prevEquipImages = new Image[(int)EquipmentType.Length];

    // 꺼놓은 상태에서 아이템을 여러개 계속 먹어 그럼 어떻게 하냐
    // prevImages 처럼 해놓으면 됮않나?

    private void OnEnable()
    {
        // 플레이어 인벤토리 구현 시 이벤트에 구독 
        inventory.OnEquipmentStored += UpdateEquipmentUI;
        inventory.OnEquipmentRemoved += HideShieldUI;

        OptionManager.Instance.OnEquipShowEnabled += StartRestoreEquipmentImages;
    }

    private void OnDisable()
    {
        // 플레이어 인벤토리 구현 시 이벤트에 구독 해제
        inventory.OnEquipmentStored -= UpdateEquipmentUI;
        inventory.OnEquipmentRemoved -= HideShieldUI;

        OptionManager.Instance.OnEquipShowEnabled -= StartRestoreEquipmentImages;
    }
    
    // 아이템 획득 시 장비 타입에 맞는 UI에 표시
    private void UpdateEquipmentUI(Vector3 pos, EquipmentData data)
    {
        // 활성화 후 이미지 설정
        Image image = equipmentImageManager.EquipmentImagePop();
        image.sprite = data.Sprite;

        // 장비 표시 옵션이 꺼져있나? 그렇다! 그럼 저장만하고 풀로 다시 돌아가!
        if (!OptionManager.Instance.ShowEquip)
        {
            // 기존 장비가 있다면 이미지 반환
            SwapEquipmentImage(data.EquipmentType, image);
            return;
        }

        // 이미지의 위치를 아이템의 위치로 바꾼 후 보간해서 이동 후 도착하면 활성화
        Transform imageTransform = equipmentImages[(int)data.EquipmentType].transform;
        
        Vector2 targetPos = imageTransform.position;

        // 스크린 좌표로 변환
        Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
        image.transform.position = screenPos;

        // 움직이는 애니메이션
        image.transform.DOMove(targetPos, UIAnimationSettings.SlowDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 기존 장비가 있다면 이미지 반환
                SwapEquipmentImage(data.EquipmentType, image);
            });
    }

    private void SwapEquipmentImage(EquipmentType type, Image newImage)
    {
        int index = (int)type;

        // 이전 이미지 반환
        if (prevEquipImages[index] != null)
        {
            equipmentImageManager.ReturnEquipmentImage(prevEquipImages[index]);
        }

        // 새 이미지 저장
        prevEquipImages[index] = newImage;
    }

    // 장비 UI도 이미지를 풀링했으므로 풀로 넣기
    private void HideShieldUI(EquipmentType type = EquipmentType.Shield)
    {
        equipmentImageManager.ReturnEquipmentImage(prevEquipImages[(int)type]); 
    }

    private void StartRestoreEquipmentImages()
    {
        StartCoroutine(RestoreEquipmentImages());
    }

    // 장비 표시가 켜졌을 때 prevImages에 데이터가 들어있다면 표시를 해주는 메서드
    private IEnumerator RestoreEquipmentImages()
    {
        yield return null; 

        for (int i = 0; i < prevEquipImages.Length; i++)
        {
            Image img = prevEquipImages[i];

            if (img == null) continue;

            img.transform.position = equipmentImages[i].transform.position;
        }
    }
}