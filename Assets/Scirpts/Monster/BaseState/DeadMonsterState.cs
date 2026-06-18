using UnityEngine;

public class DeadMonsterState : MonsterBase
{
    
    protected override void Awake()
    {
        base.Awake();
        
    }

    private void OnEnable()
    {
        DeadAnimation();
    }

    private void DeadAnimation()
    {
        rb.linearVelocity = Vector2.zero; // 죽었을 때 움직임 멈추기
        monsterCollider2D.enabled = false; // 충돌 끄기


    }

}
