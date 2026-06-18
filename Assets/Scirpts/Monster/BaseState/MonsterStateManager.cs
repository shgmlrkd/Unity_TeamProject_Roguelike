using UnityEngine;
using UnityEngine.Events;

public class MonsterStateManager : MonoBehaviour
{

    [SerializeField] private MonsterData monsterData;
    [SerializeField] private MonsterBase[] stateBeses;
    [SerializeField] private MonsterStateEnum monsterState = MonsterStateEnum.None;
    [SerializeField] private UnityEvent<MonsterStateEnum> OnstateChanged;
    [SerializeField] private LayerMask playerLayer;

    private Transform target;
    public Transform Target { get { return target; } }

    public MonsterData MonsterData {get {return monsterData;}}


    private void Start()
    {

        SetState(MonsterStateEnum.Patrol);
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
        Collider2D player = Physics2D.OverlapCircle(transform.position, monsterData.ContactRange, playerLayer);


        if (player == null)
        {
            target = null;
            SetState(MonsterStateEnum.Patrol);
            return;
            
        }


        target = player.transform; // 타겟 위치 지정
        

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= monsterData.AttakcRange)
        {
            SetState(MonsterStateEnum.Attack);
        }
        else
        {
            SetState(MonsterStateEnum.Chase);
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

    private void OnDrawGizmos() // 사거리 체크
    {
        if (monsterData == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, monsterData.ContactRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterData.AttakcRange);
    }

}
