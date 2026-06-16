using System.Collections;
using UnityEngine;

public class PatrolMonsterState : MonsterBase
{
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Coroutine patrolCo;
    private WaitForSeconds wait;

    protected override void Awake() // 시작및 목표 위치 지정
    {
        base.Awake();
        wait = new WaitForSeconds(monsterData.PatrolWaitTime);
        startPoint = transform.position;
        endPoint = startPoint + monsterData.MoveOffset;
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
            while (transform.position.x < endPoint.x)
            {
                transform.Translate(monsterData.PatrolSpeed * Time.deltaTime * Vector2.right);
                yield return null;
            }
            yield return wait;


            while (transform.position.x > startPoint.x)
            {
                transform.Translate(monsterData.PatrolSpeed * Time.deltaTime * Vector2.left);
                yield return null;

            }

            yield return wait;

        }
    }
}
