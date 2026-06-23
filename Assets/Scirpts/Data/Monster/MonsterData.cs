using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "GameData/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("몬스터 정보")]
    [SerializeField] private string monsterName;
    [SerializeField] private MonsterBase monsterPrefab;

    [Header("몬스터 능력치")]
    [Header("몬스터 체력")]
    [SerializeField] private int monsterMaxHp;
    [Header("몬스터 이동 속도")]
    [SerializeField] private float moveSpeed;
    [Header("몬스터 공격")]
    [SerializeField] private int attackDamage;
    [Header("공격 사거리")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTakeDamageRange;
    [Header("몬스터 공격 딜레이")]
    [SerializeField] private float attackDelay;
    [Header("플레이어 인식 범위")]
    [SerializeField] private float contactRange;
    [Header("정찰 속도")]
    [SerializeField] private float patrolSpeed;
    [Header("정찰 대기시간")]
    [SerializeField] private float patrolWaitTime;
    [Header("정찰 거리제한")]
    [SerializeField] private int minPatrolDistance = 3; 
    [SerializeField] private int maxPatrolDistance = 8; 
    [Header("드랍 골드")]
    [SerializeField] private int minDropGold;
    [SerializeField] private int maxDropGold;


    public string MonsterName { get { return monsterName; } }
    public MonsterBase MonsterPrefab { get { return monsterPrefab; } }
    public int MonsterMaxHp { get { return monsterMaxHp; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public int AttackDamage { get { return attackDamage; } }
    public float AttackRange { get { return attackRange; } }
    public float AttackTakeDamageRange { get { return attackTakeDamageRange; } }
    public float AttakcDelay { get { return attackDelay; } }
    public float ContactRange { get { return contactRange; } }
    public float PatrolSpeed { get {return patrolSpeed; } }
    public float PatrolWaitTime { get { return patrolWaitTime; } }
    public int MinPatrolDistance { get { return minPatrolDistance; } }
    public int MaxPatrolDistance { get { return maxPatrolDistance; } }
    public int DropGold { get { return Random.Range(minDropGold, maxDropGold); } }
   

}