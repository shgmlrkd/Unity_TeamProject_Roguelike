using UnityEngine;

public class DeadMonsterState : MonsterBase
{

    private void OnEnable()
    {
        EnterDeadState();
    }

    private void EnterDeadState()
    {

        rb.linearVelocity = Vector2.zero; // 죽었을 때 움직임 멈추기
        monsterCollider2D.enabled = false; // 충돌 끄기

    }

    // 이벤트 메서드
    //{
    //      풀링 넣고 이벤트 메서드에 연결
    //}

}
