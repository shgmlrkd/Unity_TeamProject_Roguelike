using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator refAnimator;

    private int monsterStateHash = Animator.StringToHash("MonsterState");
    private int daedTriggerHash = Animator.StringToHash("IsDaed");


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
    public void OnDaedTrigger()
    {
        refAnimator.SetTrigger(daedTriggerHash);
    }
 
    public void SetBossMonsterPattern(BossMonsterPattern bossMonsterPattern)
    {
        if (refAnimator == null) return;

        refAnimator.SetInteger("BossMonster", (int)bossMonsterPattern);
    }

    public AnimatorStateInfo GetcurrentStateInfo()
    {
        return refAnimator.GetCurrentAnimatorStateInfo(0);
    }

}
