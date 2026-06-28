using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{
    private const float FLOAT_DISTANCE = 0.25f;
    private const float SCATTER_RADIUS = 0.6f;
    private const float FULL_CIRCLE_ANGLE = 360.0f;

    private const int INFINITE_LOOP = -1;

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
    public void OldEquipmentInit(ItemData data, Vector2 pos)
    {
        Initialize(data, 0, pos);
        PlayFloatingAnimation();
    }
    
    // 아이템에 맞는 데이터와 위치로 초기화 및 애니메이션 재생
    public void DropItemInit(ItemData data, int goldAmount, Vector2 pos)
    {
        Initialize(data, goldAmount, pos);
        PlayDropAnimation(pos);
    }

    private void Initialize(ItemData data, int goldAmount, Vector2 pos)
    {
        itemData = data;
        this.goldAmount = goldAmount;
        print($"초기화 골드 : {this.goldAmount}");

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
        transform.DOKill();
        transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1.0f, UIAnimationSettings.FastDuration).SetEase(Ease.OutBack));

        seq.Join(
            transform.DOJump(targetPos, jumpPower, 1, duration)
            .SetEase(Ease.OutQuad)
        );

        seq.OnComplete(() =>
        {
            PlayFloatingAnimation();
        });
    }

    private void PlayFloatingAnimation()
    {
        transform.DOKill();
        transform.DOMoveY(transform.position.y + FLOAT_DISTANCE, UIAnimationSettings.SlowDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(INFINITE_LOOP, LoopType.Yoyo);
    }

    // 아이템 개수에 따라 원형으로 드랍하는 위치 반환
    public Vector3 GetScatterOffset(int index, int count)
    {
        if (count == 1)
        {
            return Vector3.zero;
        }

        float radius = SCATTER_RADIUS;

        float angle = (FULL_CIRCLE_ANGLE / count) * index;
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
                    transform.DOKill();

                    SoundManager.Instance.PlaySFX(SoundKey.CollectedItem);

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

                transform.DOKill();

                SoundManager.Instance.PlaySFX(SoundKey.CollectedItem);

                ItemManager.Instance.ReturnItem(this);

                return;
            }
        }

        // 골드 아이템 일 때
        if (collision.CompareTag("Player"))
        {
            InGameManager.Instance.CollectedGold(goldAmount);

            transform.DOKill();

            SoundManager.Instance.PlaySFX(SoundKey.CollectedItem);

            ItemManager.Instance.ReturnItem(this);
        }
    }
}