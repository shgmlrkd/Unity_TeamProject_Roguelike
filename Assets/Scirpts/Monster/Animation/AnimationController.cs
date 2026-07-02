using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator refAnimator;

    private readonly int hitTrigger = Animator.StringToHash("IsHit"); 
    private readonly int deadTriggerHash = Animator.StringToHash("IsDaed");
    private readonly int monsterStateHash = Animator.StringToHash("MonsterState");

    // 상태 (Idle, Chase, AttackSelect, Hit, Dead)
    private readonly int bossStateHash = Animator.StringToHash("BossState");

    // 기본 공격
    private readonly int bossBaseAttackHash = Animator.StringToHash("BaseAttack");

    // 대시 공격
    private readonly int bossDashStartHash = Animator.StringToHash("DashStart");
    private readonly int bossDashAttackHash = Animator.StringToHash("DashAttack");
    private readonly int bossDashSuccessHash = Animator.StringToHash("DashSuccess");
    private readonly int bossDashFailHash = Animator.StringToHash("DashFail"); 
    private readonly int bossStunEndHash = Animator.StringToHash("StunEnd");

    // 잡몹 스폰
    private readonly int bossSummonHash = Animator.StringToHash("Summon");

    // 보스 Hit
    private readonly int bossDeadHash = Animator.StringToHash("BossDead");

    // 투사체 공격
    private readonly  int bossFireBallBrustHash = Animator.StringToHash("FireBallBrust");

    // 페이즈 2 넘어가기 (힐 받음)
    private readonly int bossPhaseTwoHash = Animator.StringToHash("PhaseTwo");
    private readonly int bossIsPhaseTwoHash = Animator.StringToHash("IsPhaseTwo");

    private void Awake()
    {
        if (refAnimator == null)
        {
            refAnimator = GetComponent<Animator>();
        }
    }

    #region Common

    public void OnStateChanged(MonsterStateEnum monsterState)
    {
        refAnimator.SetInteger(monsterStateHash, (int)monsterState);
    }

    #endregion Common

    #region Normal Monster

    public void OnHitTrigger()
    {
        refAnimator.SetTrigger(hitTrigger);
    }

    public void OnDeadTrigger()
    {
        refAnimator.SetTrigger(deadTriggerHash);
    }

    #endregion Normal Monster

    #region Boss

    // 행동에 따른 애니메이션 변화
    public void OnStateChanged(BossStateEnum bossState)
    {
        refAnimator.SetInteger(bossStateHash, (int)bossState);
    }

    #region BaseAttack

    // 기본 공격 선택
    public void OnBossBaseAttackTrigger()
    {
        refAnimator.SetTrigger(bossBaseAttackHash);
    }

    #endregion BaseAttack

    #region DashAttack

    // 대시 공격 선택
    public void OnBossDashAttackTrigger()
    {
        refAnimator.SetTrigger(bossDashAttackHash);
    }

    // 대시 시작
    public void OnBossDashStartTrigger()
    {
        refAnimator.SetTrigger(bossDashStartHash);
    }

    // 대시 성공
    public void OnBossDashSuccess()
    {
        refAnimator.SetTrigger(bossDashSuccessHash);
    }

    // 대시 실패
    public void OnBossDashFail()
    {
        refAnimator.SetTrigger(bossDashFailHash);
    }

    // 기절 상태
    public void OnBossStunned(bool isStun)
    {
        refAnimator.SetBool(bossStunEndHash, isStun);
    }

    #endregion DashAttack

    #region Summon

    // 일반 몬스터 소환
    public void OnBossSummonTrigger()
    {
        refAnimator.SetTrigger(bossSummonHash);
    }

    #endregion Summon

    #region FireballBurst

    public void OnBossFireballBurstTrigger()
    {
        refAnimator.SetTrigger(bossFireBallBrustHash);
    }

    #endregion FireballBurst

    #region PhaseTwo

    public void OnBossPhaseTwo(bool isPhaseTwo)
    {
        refAnimator.SetBool(bossIsPhaseTwoHash, isPhaseTwo);
    }

    #endregion PhaseTwo

    #region Heal (Page 2)

    public void OnBossHealTrigger()
    {
        refAnimator.SetTrigger(bossPhaseTwoHash);
    }

    #endregion

    #region Dead

    public void OnBossDeadTringger()
    {
        refAnimator.SetTrigger(bossDeadHash);
    }

    # endregion Dead

    #endregion Boss

    public AnimatorStateInfo GetcurrentStateInfo()
    {
        return refAnimator.GetCurrentAnimatorStateInfo(0);
    }

}
