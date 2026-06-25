using DG.Tweening;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemData itemData = null;

    private EquipmentData equipmentData = null;
    private ConsumableData consumableData = null;

    private SpriteRenderer spriteRenderer;

    private float canPickupTime;

    private int goldAmount;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 장비창에 있던 아이템을 다른 장비와 스왑할 때
    public void Initialize(ItemData data, Vector2 pos)
    {
        Initialize(data, 0, pos, false);
    }
    
    // 아이템에 맞는 데이터와 위치로 초기화 및 애니메이션 재생
    public void Initialize(ItemData data, int goldAmount, Vector2 pos)
    {
        Initialize(data, goldAmount, pos, true);
    }

    private void Initialize(ItemData data, int goldAmount, Vector2 pos, bool playAnimation)
    {
        itemData = data;
        this.goldAmount = goldAmount;

        equipmentData = null;
        consumableData = null;

        if (data is EquipmentData equipment)
        {
            equipmentData = equipment;
        }
        else if (data is ConsumableData consumable)
        {
            consumableData = consumable;
        }

        transform.position = pos;

        UpdateVisual();

        if (playAnimation)
        {
            PlayDropAnimation(pos);
        }
    }

    // 아이템 스프라이트 업데이트
    private void UpdateVisual()
    {
        if (itemData == null || spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = itemData.Sprite;
    }

    // 바닥에 떨어진 아이템 줍는 딜레이 시간 세팅
    public void SetPickupDelay(float delay)
    {
        canPickupTime = Time.time + delay;
    }

    // 아이템 드랍 시 애니메이션 실행
    public void PlayDropAnimation(Vector2 targetPos, float jumpPower = 1.2f, float duration = 0.4f)
    {
        transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1.0f, UIAnimationSettings.FastDuration).SetEase(Ease.OutBack));

        seq.Join(
            transform.DOJump(targetPos, jumpPower, 1, duration)
            .SetEase(Ease.OutQuad)
        );
    }

    // 아이템 개수에 따라 원형으로 드랍하는 위치 반환
    public Vector3 GetScatterOffset(int index, int count)
    {
        if (count == 1)
        {
            return Vector3.zero;
        }

        float radius = 0.6f;

        float angle = (360.0f / count) * index;
        float rad = angle * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 아이템 줍기 쿨타임
        if (Time.time < canPickupTime)
        {
            return;
        }

        // 인벤토리가 생겼다면 이렇게 함
        if (collision.TryGetComponent(out PlayerInventory playerInventory))
        {
            if (equipmentData != null)
            {
                bool success = playerInventory.StoreEquipment(transform.position, equipmentData);

                if (success)
                {
                    ItemManager.Instance.ReturnItem(this);
                }

                return;
            }
        }

        // 소비 아이템 일 때
        if (collision.TryGetComponent(out PlayerHP playerHp))
        {
            if(consumableData != null)
            {
                playerHp.Heal(consumableData.HealAmount);
            }

            ItemManager.Instance.ReturnItem(this);

            return;
        }

        // 골드 아이템 일 때

        /* 하트 UI 표시 테스트용
        if (collision.TryGetComponent(out PlayerHP playe))
        {
            if (equipmentData != null)
            {
                playe.SetBonusHp(equipmentData.ShieldHp);
            }

            gameObject.SetActive(false);
        }*/
    }
}