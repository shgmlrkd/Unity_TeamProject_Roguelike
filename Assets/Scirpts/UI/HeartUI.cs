using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    // 하트 UI 개수
    private const int MaxHeartCount = 5;
    private const int MaxBonusHeartCount = 10;

    //하트 당 hp
    private const int HpPerHeart = 2;

    [SerializeField] 
    private PlayerHP playerHp;

    [SerializeField]
    private Transform heartContainer;

    [SerializeField]
    private Image heartPrefab;

    [Header("체력 UI 스프라이트")]
    [SerializeField] 
    private Sprite[] heartSprites;

    [Header("추가 체력 UI 스프라이트")]
    [SerializeField]
    private Sprite[] bonusHeartSprites;

    private Image[] baseHearts = new Image[MaxHeartCount];
    private Image[] bonusHearts = new Image[MaxBonusHeartCount];

    private bool isCreated;
    private void Awake()
    {
        if (playerHp == null)
        {
            playerHp = FindFirstObjectByType<PlayerHP>();
        }
        CreateHearts();
    } 

    private void OnEnable()
    {
        if (playerHp != null)
        {
            playerHp.OnHpChanged += UpdateHearts;
        }
    }

    private void OnDisable()
    {
        if (playerHp != null)
        {
            playerHp.OnHpChanged -= UpdateHearts;
        }
    }

    private void Start()
    {
        if (playerHp == null)
        {
            Debug.LogError("HeartUI: PlayerHP가 연결되지 않았습니다.");
            return;
        }

        UpdateHearts(playerHp.CurrentHp, playerHp.MaxHp, playerHp.CurrentBonusHp);
    }

    private void CreateHearts()
    {
        if (isCreated)
        {
            return;
        }
        if (heartPrefab == null || heartContainer == null)
        {
            Debug.LogError("HeartUI: heartPrefab 또는 heartContainer가 연결되지 않았습니다.");
            return;
        }
    
        // 최대 체력 개수만큼 하트를 생성
        for (int i = 0; i < MaxHeartCount; i++)
        {
            Image heart = Instantiate(heartPrefab, heartContainer);
            baseHearts[i] = heart;
        }

        // 최대 추가 체력 개수만큼 하트를 생성
        for (int i = 0; i < MaxBonusHeartCount; i++)
        {
            Image heart = Instantiate(heartPrefab, heartContainer);
            heart.gameObject.SetActive(false);

            bonusHearts[i] = heart;
        }
        isCreated = true;
    }

    private void UpdateHearts(int curHp, int maxHp, int curBonusHp)
    {
        if (!isCreated)
        {
            return;
        }
        if (!IsSpriteArrayValid())
        {
            return;
        }
        UpdateBaseHearts(curHp, maxHp);
        UpdateBonusHearts(curBonusHp);
    }
    private bool IsSpriteArrayValid()
    {
        if (heartSprites == null || heartSprites.Length <= (int)HeartType.emptyHeart)
        {
            Debug.LogError("HeartUI: heartSprites 배열 크기가 부족합니다. HeartType enum 값과 배열 Size를 확인하세요.");
            return false;
        }

        if (bonusHeartSprites == null || bonusHeartSprites.Length <= (int)HeartType.halfHeart)
        {
            Debug.LogError("HeartUI: bonusHeartSprites 배열 크기가 부족합니다. fullHeart, halfHeart 스프라이트가 필요합니다.");
            return false;
        }

        return true;
    }

    private void UpdateBaseHearts(int curHp, int maxHp)
    {
        int fullHeartCount = curHp / HpPerHeart;
        int halfHeartCount = curHp % HpPerHeart;

        // 최대 체력에 따라 활성화(잠금 해제)된 하트의 개수를 계산
        int maxActiveHearts = maxHp / HpPerHeart;

        // 최대 체력이 홀수면 최대 하트 개수 1 증가
        if (maxHp % HpPerHeart != 0) 
        {
            maxActiveHearts++;
        }

        for (int i = 0; i < MaxHeartCount; i++)
        {
            // 이 하트는 활성화된 하트
            if (i < maxActiveHearts) 
            {
                if (i < fullHeartCount)
                {
                    baseHearts[i].sprite = heartSprites[(int)HeartType.fullHeart];
                }
                else if (i == fullHeartCount && halfHeartCount > 0) 
                {
                    baseHearts[i].sprite = heartSprites[(int)HeartType.halfHeart];
                }
                else
                {
                    baseHearts[i].sprite = heartSprites[(int)HeartType.emptyHeart];
                }
            }
            else // 잠긴 하트
            {
                baseHearts[i].sprite = heartSprites[(int)HeartType.lockHeart];
            }
        }
    }

    private void UpdateBonusHearts(int curBonusHp)
    {
        int fullBonusHeartCount = curBonusHp / HpPerHeart;
        int halfBonusHeartCount = curBonusHp % HpPerHeart;

        for (int i = 0; i < MaxBonusHeartCount; i++)
        {
            // 사용 중인 보너스 하트는 활성화
            if (i < fullBonusHeartCount)
            {
                bonusHearts[i].gameObject.SetActive(true);
                bonusHearts[i].sprite = bonusHeartSprites[(int)HeartType.fullHeart];
            }
            else if (i < fullBonusHeartCount + halfBonusHeartCount)
            {
                bonusHearts[i].gameObject.SetActive(true);
                bonusHearts[i].sprite = bonusHeartSprites[(int)HeartType.halfHeart];
            }
            // 사용하지 않는 보너스 하트는 비활성화
            else
            {
                bonusHearts[i].gameObject.SetActive(false);
            }
        }
    }
}