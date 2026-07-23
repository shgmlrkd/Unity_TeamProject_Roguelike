using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BossStatePair
{
    public BossStateEnum state;
    public BossBase bossbase;
}

public class BossStateManager : MonoBehaviour
{
    [SerializeField]
    private BossStatePair[] statePairs;

    private readonly Dictionary<BossStateEnum, BossBase> stateDict = new Dictionary<BossStateEnum, BossBase>();

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform firePos;

    [SerializeField]
    private Transform attackPos;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private BossMonsterData data;

    [SerializeField]
    private AnimationController animController;

    [SerializeField]
    private BossVisualController bossVisual;

    [SerializeField] 
    private UnityEvent<BossStateEnum> OnstateChanged;

    private BossContext context;
    public BossContext Context => context;
    
    [SerializeField] 
    private BossStateEnum bossState = BossStateEnum.None;

    public BossStateEnum BossState => bossState;
    public BossVisualController VisualController => bossVisual;

    private void Awake()
    {
        context = new BossContext();

        context.target = target;
        context.firePos = firePos;
        context.attackPos = attackPos;
        context.rb = GetComponent<Rigidbody2D>();
        context.data = data;
        context.animController = GetComponentInChildren<AnimationController>();
        context.Initialize();

        bossVisual.BindContext(context);

        InitializeStates();
    }

    private void InitializeStates()
    {
        // 상태 등록 및 초기화
        foreach (BossStatePair pair in statePairs)
        {
            // 상태 스크립트가 없으면 건너뜀
            if (pair.bossbase == null)
            {
                continue;
            }

            // 동일한 상태가 중복 등록되는 것을 방지
            if (!stateDict.TryAdd(pair.state, pair.bossbase))
            {
                continue;
            }

            pair.bossbase.Init(this, context);
        }
    }

    private void Update()
    {
        if (bossState == BossStateEnum.None)
            return;

        if(stateDict.TryGetValue(bossState, out BossBase state))
        {
            state.ManualUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (bossState == BossStateEnum.None)
            return;

        if (stateDict.TryGetValue(bossState, out BossBase state))
        {
            state.FixedTick();
        }
    }

    // 상태 변환
    public void SetState(BossStateEnum next)
    {
        if (bossState == next) return;

        // 현재 상태 종료
        if (bossState != BossStateEnum.None &&
            stateDict.TryGetValue(bossState, out BossBase currentState))
        {
            currentState.Exit();
        }

        // 다음 상태가 등록된 상태인지 확인
        if (!stateDict.TryGetValue(next, out BossBase nextState))
        {
            return;
        }

        bossState = next;

        nextState.Enter();

        OnstateChanged?.Invoke(bossState);
    }
}

