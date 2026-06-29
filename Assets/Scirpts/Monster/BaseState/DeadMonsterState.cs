using System.Collections;
using UnityEngine;

public class DeadMonsterState : MonsterBase
{
    private float fabeSpeed = 2.0f;         // 줄어드는 속도
    private float disableAlpha = 0.05f;     // 값 이하가 되면 완전히 사라짐
    private bool isItemDrop = false;

    private Coroutine fadeCoroutine;
    private Quaternion visualRootStartRotation; // Root의 원래 회전값 저장용

    protected override void Awake()
    {
        base.Awake();
        // Root의 원래 회전값 저장
        // Skeleton 자식 Root를 인스펙터에 연결해야 함
        if (monsterStateManager.VisualRoot != null)
        {
            visualRootStartRotation = monsterStateManager.VisualRoot.localRotation;
        }

    }

    protected override void OnEnable()
    {

        isItemDrop = false;
        DeadStart(); // 죽었을때 이동 정지 / 충돌 끄기
        controller.OnDeadTrigger();
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
            ReturnToPool();
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

            rb.linearVelocity = Vector3.zero;
            yield return null;
        }
         
        fadeCoroutine = null;
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        // 풀에 들어가기 전에 Root 회전을 먼저 복구
        // 죽은 상태의 기울어진 회전값이 남는 것을 방지
        ResetVisualRootRotation();

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

        // 풀에서 나왔을때 다시 충돌 켜기
        if(monsterCollider2D != null)
        {
            monsterCollider2D.enabled = true;
        }

        ResetVisualRootRotation();

    }
    private void ResetVisualRootRotation()
    {
        if (monsterStateManager.VisualRoot == null)
        {
            return;
        }

        // 저장해둔 Root의 원래 회전값으로 복구
        monsterStateManager.VisualRoot.localRotation = visualRootStartRotation;
    }

}
