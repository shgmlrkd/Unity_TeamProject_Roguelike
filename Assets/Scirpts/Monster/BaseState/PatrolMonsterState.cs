using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class PatrolMonsterState : MonsterBase
{
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Coroutine patrolCo;
    private WaitForSeconds wait;
    Vector2 nextPosition;



    protected override void Awake() // 시작및 목표 위치 지정
    {
        base.Awake();
        wait = new WaitForSeconds(monsterStateManager.MonsterData.PatrolWaitTime);
        startPoint = transform.position;
        endPoint = startPoint + monsterStateManager.MonsterData.MoveOffset;
    }

   

    private void OnEnable() // 코루틴 시작 및 if문을 위해서 저장
    {
    
        patrolCo = StartCoroutine(PatrolCo());
        
    }

    private void OnDisable() // 코루틴 종료
    {
        if(patrolCo != null)
        {
            StopCoroutine(patrolCo);
            patrolCo = null;
        }
    }

    private IEnumerator PatrolCo()  // 좌우 반복 이동
    {
        while(true)
        {
            while (rb.position.x < endPoint.x)
            {
                nextPosition = Vector2.MoveTowards(rb.position, endPoint, monsterStateManager.MonsterData.PatrolSpeed * Time.fixedDeltaTime);
                rb.MovePosition(nextPosition);
                yield return null;
            }
            yield return wait;


            while (rb.position.x > startPoint.x)
            {

                nextPosition = Vector2.MoveTowards(rb.position, startPoint, monsterStateManager.MonsterData.PatrolSpeed * Time.fixedDeltaTime);
                rb.MovePosition(nextPosition);
                yield return null;

            }

            yield return wait;

        }
    }
}
