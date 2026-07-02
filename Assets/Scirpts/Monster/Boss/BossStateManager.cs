using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BossStateManager : MonoBehaviour
{
    [SerializeField]
    private BossBase[] states;

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

        // 상태 매니저와 타겟(플레이어) 각 상태 스크립트에서 초기화
        foreach (BossBase state in states)
        {
            state.Init(this, context);
        }
    }

    private void OnEnable()
    {
        SetState(BossStateEnum.Idle);
    }

    private void Update()
    {
        states[(int)bossState].ManualUpdate();
    }

    private void FixedUpdate()
    {
        states[(int)bossState].FixedTick();
    }

    public void SetState(BossStateEnum next)
    {
        if (bossState == next) return;

        if (bossState != BossStateEnum.None)
        {
            states[(int)bossState].Exit();
        }

        bossState = next;

        states[(int)bossState].Enter();
        OnstateChanged?.Invoke(bossState);
    }
}