using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator refAnimator;

    private int monsterStateHash = Animator.StringToHash("MonsterState");
    private int bossmonsterStateHash = Animator.StringToHash("BossMonster");
    private int deadTriggerHash = Animator.StringToHash("IsDaed");
    private int hitTrigger = Animator.StringToHash("IsHit");

    private void Awake()
    {
        if (refAnimator == null)
        {
            refAnimator = GetComponent<Animator>();
        }
    }

    public void OnStateChanged(MonsterStateEnum monsterState)
    {
        refAnimator.SetInteger(monsterStateHash, (int)monsterState);
    }
    public void OnDeadTrigger()
    {
        refAnimator.SetTrigger(deadTriggerHash);
    }
 
    public void OnBossMonsterPattern(BossMonsterPattern bossMonsterPattern)
    {
        if (refAnimator == null) return;

        refAnimator.SetInteger(bossmonsterStateHash, (int)bossMonsterPattern);
    }

    public void OnHitTrigger()
    {
        refAnimator.SetTrigger(hitTrigger);
    }

    public AnimatorStateInfo GetcurrentStateInfo()
    {
        return refAnimator.GetCurrentAnimatorStateInfo(0);
    }

}
