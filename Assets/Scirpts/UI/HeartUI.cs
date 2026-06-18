using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    //하트 당 hp
    private const int HpPerHeart = 2;

    [SerializeField] private PlayerHP playerHp;
    [SerializeField] private Transform heartContainer;
    [SerializeField] private Image heartPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite halfHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    private readonly List<Image> hearts = new List<Image>();

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
        CreateHearts(playerHp.MaxHp);
        UpdateHearts(playerHp.CurrentHp, playerHp.MaxHp);
    }

    private void CreateHearts(int maxHp)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            Destroy(hearts[i].gameObject);
        }

        hearts.Clear();

        int heartCount = Mathf.CeilToInt(maxHp / (float)HpPerHeart);

        for (int i = 0; i < heartCount; i++)
        {
            Image heart = Instantiate(heartPrefab, heartContainer);
            heart.sprite = fullHeartSprite;
            hearts.Add(heart);
        }
    }

    private void UpdateHearts(int currentHp, int maxHp)
    {
        int heartCount = Mathf.CeilToInt(maxHp / (float)HpPerHeart);

        if (hearts.Count != heartCount)
        {
            CreateHearts(maxHp);
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartHpValue = currentHp - (i * HpPerHeart);

            if (heartHpValue >= 2)
            {
                hearts[i].sprite = fullHeartSprite;
            }
            else if (heartHpValue == 1)
            {
                hearts[i].sprite = halfHeartSprite;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
            }
        }
    }
}