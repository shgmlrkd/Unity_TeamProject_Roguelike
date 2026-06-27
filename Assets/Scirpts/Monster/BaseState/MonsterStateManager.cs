using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonsterStateManager : MonoBehaviour
{

    [Header("몬스터 Root")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private MonsterBase[] stateBeses;
    [SerializeField] private MonsterStateEnum monsterState = MonsterStateEnum.None;
    [SerializeField] private UnityEvent<MonsterStateEnum> OnstateChanged;
    [SerializeField] private LayerMask PlayerLayer;



    private float attackRangeLostTime;
    private bool isStartCheckState = false;
    private SpriteRenderer[] spriteRenderers; // 자식까지 포함한 모든 spriteRenderer
    private Transform target;
    private WaitForSeconds waitForCheckState = new WaitForSeconds(1.0f);
    private Color[] originColor;          // 풀링 재사용시 원래 색상 복구용
    private Vector3 monsterScale;
    AStarPathFinder pathFinder = null;

    public Transform VisualRoot => visualRoot;
    public SpriteRenderer[] SpriteRenderers => spriteRenderers;
    public Color[] OriginColor => originColor;
    public bool IsStartCheckState => isStartCheckState;
    public MonsterStateEnum CurrentState => monsterState;
    public Transform Target { get { return target; } }
    public MonsterData MonsterData {get {return monsterData;}}
    public AStarPathFinder PathFinder {get {return pathFinder;}}


    private void Awake()
    {
        // 루트에 spriteRenderer가 없을수 있으므로 자식까지 포함해서 모든 랜더러 가져오기
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        InitOriginColors();
        monsterScale = transform.localScale;
    }
   
    private void OnEnable()
    {
        pathFinder = null;
        isStartCheckState = false;
        ResetColor();
        SetState(MonsterStateEnum.Idle);
        StartCoroutine(WaitForCheck());
    }
    private void FixedUpdate()
    {
        if (!isStartCheckState) return;
        CheckState();
    }

    private IEnumerator WaitForCheck()
    {
        yield return waitForCheckState;
        isStartCheckState = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)   // 트리거로 노드 받기
    {
        if (collision.TryGetComponent(out AStarPathFinder pathFinder))
        {
            this.pathFinder = pathFinder;
        }  
    }

    private void CheckState() // 플레이어 감지및 행동 지정
    {

        if(monsterState == MonsterStateEnum.Dead)
        {
            return;
        }

        if(monsterState == MonsterStateEnum.Hit)
        {
            return;
        }

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
    private void InitOriginColors()
    {
        originColor = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originColor[i] = spriteRenderers[i].color;
        }
    }
    public void ResetColor() // 색상 되돌리기
    {
        if (spriteRenderers == null || originColor == null) // 없다면 리턴
        {
            return;
        }

        for (int i = 0; i < spriteRenderers.Length; i++) // 색상 되돌리기
        {
            spriteRenderers[i].color = originColor[i];
        }

    }
    public void SetState(MonsterStateEnum newState) // 상태 교체
    {

        if (monsterState == newState) return;

        if ( monsterState != MonsterStateEnum.None)
        {
            stateBeses[(int)monsterState].enabled = false;
        }
        monsterState = newState;
        stateBeses[(int) newState].enabled = true;
        OnstateChanged?.Invoke(monsterState);
    }

    public void FlipTo()
    {
        Vector3 scale = monsterScale;
        Vector3 dir = transform.position - target.position;
        dir.Normalize();

        scale.x = dir.x > 0 ? 1 : -1;

        transform.localScale = scale;
    }

    public void FlipTo(Vector3 pos)
    {
        Vector3 scale = monsterScale;
        Vector3 dir = transform.position - pos;
        dir.Normalize();

        scale.x = dir.x > 0 ? 1 : -1;

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
