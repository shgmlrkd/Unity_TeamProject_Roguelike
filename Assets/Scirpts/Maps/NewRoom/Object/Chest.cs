using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Chest : MonoBehaviour
{
    [Header("돈 소모치")]
    public int minCost = 20; // 최소 소모 비용
    public int maxCost = 60; // 최대 소모 비용
    private int fixedCost;

    public List<EquipmentData> itemPool; // 인스펙터에서 아이템들을 넣어주세요
    public GameObject interactionUI;
    public GameObject insufficientGoldUI;
    private bool isOpened = false;
    private bool isPlayerInRange = false;

    private Animator animator;
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (interactionUI != null) interactionUI.SetActive(false);
        insufficientGoldUI.SetActive(false);
    }
    private void Start()
    {
        int randomValue = Random.Range(minCost, maxCost + 1);

        fixedCost = Mathf.Clamp(randomValue, minCost, maxCost);
    }
    private void Update()
    {
        if (isPlayerInRange && !isOpened && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenChest();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player")) isPlayerInRange = true;
        if (interactionUI != null && !isOpened) interactionUI.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInRange = false;
        if (interactionUI != null) interactionUI.SetActive(false);
    }
    private void OpenChest()
    {
        if (isOpened) return;
        // 재화 보유량 체크 (GameDataManager.Instance 사용 예시)
        if (!InGameManager.Instance.UseGold(fixedCost))
        {
            StartCoroutine(ShowInsufficientMessage());
            return;
        }
        //상자 열기 로직 진행중
        isOpened = true;
        if (interactionUI != null) interactionUI.SetActive(false);
        if (animator != null) animator.SetTrigger("Open");
        // 랜덤으로 아이템 선택
        int randomIndex = Random.Range(0, itemPool.Count);
        EquipmentData selectedItem = itemPool[randomIndex];

        // 아이템 생성 (상자 위치에서 살짝 위로)
        Vector3 itemPos = transform.position + Vector3.up * 0.5f;
        ItemManager.Instance.DropItem(selectedItem, itemPos);

        StartCoroutine(FadeOutAndDestroy(2.0f));
    }
    private IEnumerator FadeOutAndDestroy(float duration)
    {
        if (sr == null) yield break;

        Color startColor = sr.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
    private IEnumerator ShowInsufficientMessage()
    {
        if (insufficientGoldUI != null)
        {
            insufficientGoldUI.SetActive(true); 
            yield return new WaitForSeconds(1.5f);
            insufficientGoldUI.SetActive(false);
        }
    }

}
