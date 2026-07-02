using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BossContext
{
    private const int PHASE_TWO_ATTACK_BONUS = 2;
    private const int PHASE_TWO_MONSTER_COUNT = 8;
    private const int FIREBALL_COUNT = 20;

    private const float PHASE_TWO_MOVE_SPEED_BONUS = 1.0f;
    private int monsterCount = 5;

    public Rigidbody2D rb;
    public Transform target;
    public Transform attackPos;
    public Transform firePos;
    public BossMonsterData data;
    public AnimationController animController;

    // 필요한 이벤트
    public Action OnHealRequested;
    public Action OnPhaseTwoRequested;
    public Action<SpriteLibraryAsset> OnSpriteLibraryChanged;
    public Action<float, float> OnFadeRequested;
    public Action<bool> OnInvincibleChanged;

    public float CurrentMoveSpeed { get; private set; }
    public int CurrentAttackDamage { get; private set; }
    public int FireballCount => FIREBALL_COUNT;
    public int SummonMonsterCount => monsterCount;
    public bool IsPhaseTwo { get; private set; } = false;

    public void Initialize()
    {
        CurrentAttackDamage = data.AttackDamage;
        CurrentMoveSpeed = data.MoveSpeed;
    }
    
    public void EnterPhaseTwo()
    {
        if (IsPhaseTwo) return;

        IsPhaseTwo = true;

        // 입반 몹 스폰
        // Phase 1 -> 5 마리
        // Phase 2 -> 8 마리
        monsterCount = PHASE_TWO_MONSTER_COUNT;

        // 공격력
        // Phase 1 -> 3 데미지
        // Phase 2 -> 5 데미지
        CurrentAttackDamage = 0;// += PHASE_TWO_ATTACK_BONUS;
        
        // 속도
        // Phase 1 -> 2
        // Phase 2 -> 3
        CurrentMoveSpeed += PHASE_TWO_MOVE_SPEED_BONUS;

        OnPhaseTwoRequested?.Invoke();
    }
}