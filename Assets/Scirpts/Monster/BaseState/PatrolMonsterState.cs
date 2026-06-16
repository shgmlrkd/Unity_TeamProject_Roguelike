using System.Collections;
using UnityEngine;

public class PatrolMonsterState : MonsterBase
{
    [Header("정찰 속도")]
    [SerializeField] private float patrolSpeed = 1.0f;
    [Header("정찰 간격")]
    [SerializeField] private Vector2 moveOffset = new Vector2(3.0f, 0.0f);
    [Header("정지 시간")]
    [SerializeField] private float waitTime = 1.0f;


    private Vector2 startPoint;
    private Vector2 endPoint;
    private Coroutine patrolCo;

    protected override void Awake() // 시작및 목표 위치 지정
    {
        base.Awake();
        startPoint = transform.position;
        endPoint = startPoint + moveOffset;
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
                transform.Translate(patrolSpeed * Time.deltaTime * Vector2.right);
                yield return null;
            }
                yield return new WaitForSeconds(waitTime);
           
            while (transform.position.x > startPoint.x)
            {
                transform.Translate(patrolSpeed * Time.deltaTime * Vector2.left);
                yield return null;

            }

            yield return new WaitForSeconds(waitTime);

        }
    }
}
