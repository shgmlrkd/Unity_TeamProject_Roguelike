using UnityEngine;
using UnityEngine.Events;

public class MonsterStateManager : MonoBehaviour
{

    [SerializeField] private MonsterData monsterData;
    [SerializeField] private MonsterBase[] stateBeses;
    [SerializeField] private AStarPathFinder pathFinder;
    [SerializeField] private MonsterStateEnum monsterState = MonsterStateEnum.None;
    [SerializeField] private UnityEvent<MonsterStateEnum> OnstateChanged;
    [SerializeField] private LayerMask PlayerLayer;

    private float attackRangeLostTime;
    private Transform target;
    private Vector3 monsterScale;

    public Transform Target { get { return target; } }
    public MonsterData MonsterData {get {return monsterData;}}
    public AStarPathFinder PathFinder {get {return pathFinder;}}

    private void Awake()
    {
        monsterScale = transform.localScale;
    }
    private void Start()
    {
        SetState(MonsterStateEnum.Idle);
    }

    private void OnEnable()
    {
        
    }
    private void FixedUpdate()
    {
        CheckState();
    }
    
    private void CheckState() // 플레이어 감지및 행동 지정
    {

        Collider2D player = Physics2D.OverlapCircle(transform.position, monsterData.ContactRange, PlayerLayer);


        if (player == null)
        {
            target = null;

            if (monsterState == MonsterStateEnum.Idle)
                return;

            if (monsterState != MonsterStateEnum.Patrol)
                SetState(MonsterStateEnum.Patrol);

            return;
        }

        target = player.transform; // 타겟 위치 지정

        FlipTo();

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= monsterData.AttackRange)
        {
            attackRangeLostTime = 0f;

            SetState(MonsterStateEnum.Attack);
        }
        else
        {
            attackRangeLostTime += Time.deltaTime;

            if (attackRangeLostTime >= 0.1f)
            {
                SetState(MonsterStateEnum.Chase);
            }
        }



    }
    public void SetState(MonsterStateEnum newState) // 상태 교체
    {
        if (monsterState == newState) return;
        if( monsterState != MonsterStateEnum.None)
        {
            stateBeses[(int)monsterState].enabled = false;
        }
        stateBeses[(int) newState].enabled = true;
        monsterState = newState;
        OnstateChanged?.Invoke(monsterState);
    }
  
    public void FlipTo()
    {
        Vector3 scale = monsterScale;
        float directionX = transform.position.x - target.position.x;

        scale.x = directionX > 0 ? 1 : -1;

        transform.localScale = scale;
    }

    public void FlipTo(Vector3 pos)
    {
        Vector3 scale = monsterScale;
        float directionX = transform.position.x - pos.x;

        scale.x = directionX > 0 ? 1 : -1;

        transform.localScale = scale;
    }

    private void OnDrawGizmos() // 사거리 체크
    {
        if (monsterData == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, monsterData.ContactRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterData.AttackRange);

        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position, monsterData.AttackTakeDamageRange);
    }

}
