using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator refAnimator;


    private void Awake()
    {
        if (refAnimator == null)
        {
            refAnimator = GetComponent<Animator>();
        }
    }

    public void OnStateChanged(MonsterStateEnum monsterState)
    {
        if (refAnimator == null) return;

        refAnimator.SetInteger("MonsterState", (int)monsterState);
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
