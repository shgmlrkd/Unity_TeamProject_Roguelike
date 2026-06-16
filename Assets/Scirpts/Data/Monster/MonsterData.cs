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
    [SerializeField] private float attakcRange;
    [Header("몬스터 공격 딜레이")]
    [SerializeField] private float attackDelay;
    [Header("플레이어 인식 범위")]
    [SerializeField] private float contactRange;

    public string MonsterName { get { return monsterName; } }
    public MonsterBase MonsterPrefab { get { return monsterPrefab; } }
    public int MonsterMaxHp { get { return monsterMaxHp; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public int AttackDamage { get { return attackDamage; } }
    public float AttakcRange { get { return attakcRange; } }
    public float AttakcDelay { get { return attackDelay; } }
    public float ContactRange { get { return contactRange; } }
}