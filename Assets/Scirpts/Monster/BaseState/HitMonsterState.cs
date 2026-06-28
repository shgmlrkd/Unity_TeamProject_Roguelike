using System.Collections;
using UnityEngine;

public class HitMonsterState : MonsterBase
{
    private Color hitColor = Color.red;
    private float hitTime = 0.1f;

    private Coroutine hitCoroutine;

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
        
        if (spriteRenderers == null || spriteRenderers.Length == 0) // spriteRenderers 가 없다면 상태 복귀
        {
            yield break;
        }

        SetHitColor(); // 색상 빨간색 변경 메서드

        yield return new WaitForSeconds(hitTime);

        monsterStateManager.ResetColor();

        hitCoroutine = null;
    }

    private void SetHitColor()
    {
        for (int i = 0; i < spriteRenderers.Length - 1; i++)   // 빨간색으로 변경
        {
            if (spriteRenderers[i] == null) // spriteRenderers 없다면 다시
            {
                continue;
            }

            spriteRenderers[i].color = hitColor;  // 색상 변경

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
