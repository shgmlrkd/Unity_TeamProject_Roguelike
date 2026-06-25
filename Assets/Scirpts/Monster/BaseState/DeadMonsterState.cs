using System.Collections;
using UnityEngine;

public class DeadMonsterState : MonsterBase
{
    private float fabeSpeed = 2.0f;         // 줄어드는 속도
    private float disableAlpha = 0.05f;     // 값 이하가 되면 완전히 사라짐
    private bool isItemDrop = false;

    private SpriteRenderer[] spriteRenderers; // 자식까지 포함한 모든 spriteRenderer
    private Color[] originColor;          // 풀링 재사용시 원래 색상 복구용
    private Coroutine fadeCoroutine;
    


    protected override void Awake()
    {
        base.Awake();
        // 루트에 spriteRenderer가 없을수 있으므로 자식까지 포함해서 모든 랜더러 가져오기
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // spriteRenderers가 하나도 없으면 color 접근에 에러나니까 방어
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            return;
        }

        // 각 SpriteRenderers의 원래 색을 저장
        originColor = new Color[spriteRenderers.Length];

        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            originColor[i] = spriteRenderers[i].color;
        }

    }

    protected override void OnEnable()
    {

        isItemDrop = false;
        DeadStart(); // 죽었을때 이동 정지 / 충돌 끄기
        controller.OnDaedTrigger();
    }

    private void DeadStart()
    {
        rb.linearVelocity = Vector2.zero; // 죽었을 때 움직임 멈추기

        monsterCollider2D.enabled = false; // 충돌 끄기
    }

    // 죽음 애니메이션 마지막에 추가할 이벤트 메서드
    public void DeadAnimatiEnd()
    {
        // 예시로 몬스터가 죽었을 때
       

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOutCo());
    }

    private IEnumerator FadeOutCo()
    {
        // spriteRenderers가 없다면 페이드 처리 불가능
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            PoolManager.Instance.ReturnPool(monsterStateManager);
            yield break;
        }
        float alpha = 1.0f;

        while (alpha > disableAlpha)  // 알파값이 정해둔 수치 이하가 될때 까지 반복
        {
            alpha = Mathf.Lerp(alpha, 0.0f, fabeSpeed * Time.deltaTime); // 알파 값을 0에 가깝게 줄이기.

            if (alpha < 0.4f && !isItemDrop)
            {
                isItemDrop = true;
                ItemManager.Instance.DropItem(monsterStateManager.MonsterData.DropItemCount,
                                              monsterStateManager.MonsterData.DropGold,
                                              transform.position);
            }

            for(int i = 0; i < spriteRenderers.Length; i++) // 모든 spriteRenderers에 값 적용
            {
                if(spriteRenderers[i] == null)
                {
                    continue;
                }

                Color color = spriteRenderers[i].color;
                color.a = alpha;
                spriteRenderers[i].color = color;

            }

            yield return null;
        }
         
        fadeCoroutine = null;
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnPool(monsterStateManager);
    }

    private void OnDisable()
    {
        // 풀로 돌아갈때 코루틴 남아있으면 정지
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // 풀에서 나왔을 때 투명한 상태로 나오지 않게 원래 색으로 복구
        if (spriteRenderers != null && originColor != null)
        {
            for(int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null)
                {
                    continue;
                }

                spriteRenderers[i].color = originColor[i];
            }
        }

        // 풀에서 나왔을때 다시 충돌 켜기
        if(monsterCollider2D != null)
        {
            monsterCollider2D.enabled = true;
        }

    }

}
