using UnityEngine;
using UnityEngine.Events;

public class MonsterStateManager : MonsterBase
{

    [SerializeField] private MonsterStateEnum monsterState = MonsterStateEnum.None;
    [SerializeField] private MonsterBase[] stateBeses;
    [SerializeField] private UnityEvent<MonsterStateEnum> OnstateChanged;
    [SerializeField] private LayerMask playerLayer;



    private void Start()
    {
        SetState(MonsterStateEnum.Patrol);
    }

    private void OnEnable()
    {
        
    }
    private void Update()
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

        target = player.transform; // 플레이어 찾기

        float distance = Vector2.Distance(transform.position, target.transform.position);

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

    
}
