using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;

public class BossVisualController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private BossStateManager stateManager;

    [SerializeField]
    private SpriteLibrary spriteLibrary;

    private const int SORTING_SCALE = 100;
    private const float HIT_FLASH_DURATION = 0.1f;

    private BossContext context;

    private SortingGroup sortingGroup;
    private SpriteRenderer spriteRenderer;

    private Coroutine fadeRoutine;
    private Coroutine flashRoutine;

    private Vector3 bossScale;
    private Color originColor;

    public Action<SpriteLibraryAsset> OnSpriteLibraryChanged;

    // 보스 플립 못하게 막을 행동들
    private static readonly BossStateEnum[] noFlipStates =
    {
        BossStateEnum.BaseAttack,
        BossStateEnum.DashAttack,
        BossStateEnum.Summon,
        BossStateEnum.PhaseTransition,
        BossStateEnum.ProjectileAttack,
        BossStateEnum.Dead
    };

    private void Awake()
    {
        sortingGroup = GetComponentInChildren<SortingGroup>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originColor = spriteRenderer.color;
        bossScale = transform.localScale;
    }

    private void OnDisable()
    {
        if (context == null) return;

        context.OnFadeRequested -= FadeRequested;
        context.OnSpriteLibraryChanged -= ChangeSpriteLibrary;
    }

    private void Update()
    {
        // 레이어 계산
        SetOrderInLayer();

        if (CanFlip())
        {
            // 플립
            FlipTo();
        }
    }

    private void SetOrderInLayer()
    {
        sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * SORTING_SCALE);
    }

    private void FlipTo()
    {
        Vector3 scale = bossScale;
        Vector3 dir = transform.position - target.position;
        dir.Normalize();

        scale.x = dir.x < 0 ? 1 : -1;

        transform.localScale = scale;
    }

    private bool CanFlip()
    {
        bool canFlip = !noFlipStates.Contains(stateManager.BossState);

        return canFlip;
    }

    private void FadeRequested(float targetAlpha, float duration)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeOutCoroutine(targetAlpha, duration));
    }

    private IEnumerator FadeOutCoroutine(float targetAlpha, float duration)
    {
        float startAlpha = spriteRenderer.color.a;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration));

            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
    }

    private void ChangeSpriteLibrary(SpriteLibraryAsset asset)
    {
        spriteLibrary.spriteLibraryAsset = asset;
    }

    private IEnumerator HitFlashCoroutine()
    {
        Color original = spriteRenderer.color;

        spriteRenderer.color = Color.red;

        float timer = 0.0f;

        while (timer < HIT_FLASH_DURATION)
        {
            timer += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(Color.red, original, timer / HIT_FLASH_DURATION);
            yield return null;
        }

        spriteRenderer.color = original;
    }

    public void BindContext(BossContext context)
    {
        this.context = context;
        this.context.OnFadeRequested += FadeRequested;
        this.context.OnSpriteLibraryChanged += ChangeSpriteLibrary;
    }

    public void PlayHitFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(HitFlashCoroutine());
    }
}