using System.Collections;
using UnityEngine;

public class HitMonsterState : MonsterBase
{
    [Header("적용할 색상 Root")]
    [SerializeField] private Transform visualRoot;
    private Color hitColor = Color.red;
    private float hitTime = 0.1f;

    private SpriteRenderer[] spriteRenderers;   // 자식까지 포함해서 모든 랜더러
    private Color[] originColor;                // 원래 색 저장 (스프라이트가 배열이니까 배열로)
    private Coroutine hitCoroutine;

    protected override void Awake()
    {
        base.Awake();

        if(visualRoot != null )
        {
            spriteRenderers = visualRoot.GetComponentsInChildren<SpriteRenderer>(true); // 자식 포함해서 가져오기
        }

        
        if(spriteRenderers == null || spriteRenderers.Length == 0 ) // 랜더러가 없다면 색상 저장 X
        {
            return;
        }

        originColor = new Color[spriteRenderers.Length]; // 원래 색 저장

        for(int i = 0; i < spriteRenderers.Length; i++)  
        {
            originColor[i] = spriteRenderers[i].color;
        }

    }

    protected override void OnEnable()
    {

        controller.OnHitTrigger();

        if (hitCoroutine != null) // 이전 코루틴이 남아 있으면 정지
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        hitCoroutine = StartCoroutine(HitColorco()); // 피격 효과 시작

    }


    private IEnumerator HitColorco()
    {
        if(spriteRenderers == null || spriteRenderers.Length == 0) // spriteRenderers 가 없다면 상태 복귀
        {
            yield break;
        }

        SetHitColor(); // 색상 빨간색 변경 메서드

        yield return new WaitForSeconds(hitTime);

        ResetColor();

        hitCoroutine = null;
    }

    private void SetHitColor()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)   // 빨간색으로 변경
        {
            if (spriteRenderers[i] == null) // spriteRenderers 없다면 다시
            {
                continue;
            }

            Color color = hitColor;

            color.a = spriteRenderers[i].color.a; // 기존 알파값 유지하기

            spriteRenderers[i].color = color;  // 색상 변경

        }
    }

    private void ResetColor() // 색상 되돌리기 메서드
    {
        if(spriteRenderers == null || originColor == null) // 없다면 리턴
        {
            return;
        }

        for (int i = 0; i < spriteRenderers.Length; i++) // 색상 되돌리기
        {
            spriteRenderers[i].color = originColor[i];
        }

    }

    public void AnimaEndHitState()
    {
        // ResetColor(); // 색이 남아 있을수도 있으니까 한번더

        // Hit가 끝나면 Idle로 돌려놓고 이 후 정찰 추적 등 다시 판단하기
        monsterStateManager.SetState(MonsterStateEnum.Patrol); 

    }

    private void OnDisable()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

    }


}
