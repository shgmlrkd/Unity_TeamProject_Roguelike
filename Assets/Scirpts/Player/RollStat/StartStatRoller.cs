using UnityEngine;
using UnityEngine.SceneManagement;

public class StartStatRoller : MonoBehaviour
{//시작 버튼에 연결 필요
    //랜덤 범위 지정
    [Header("HP Range")]
    [SerializeField] private int minMaxHp = 1;
    [SerializeField] private int maxMaxHp = 5;

    [Header("Attack Range")]
    [SerializeField] private int minAttack = 1;
    [SerializeField] private int maxAttack = 3;

    [Header("Attack Speed Rate Range")]
    [SerializeField] private float minAttackSpeedRate = -0.2f;
    [SerializeField] private float maxAttackSpeedRate = 0.3f;

    [Header("Move Speed Rate Range")]
    [SerializeField] private float minMoveSpeedRate = -0.2f;
    [SerializeField] private float maxMoveSpeedRate = 0.3f;

    [Header("Shield HP Range")]
    [SerializeField] private int minShieldHp = 0;
    [SerializeField] private int maxShieldHp = 2;

    public void RollStats()
    {
        PlayerRolledStat rolledStat = new PlayerRolledStat
        {
            //랜덤 돌림
            MaxHp = 2*(Random.Range(minMaxHp, maxMaxHp + 1)),
            Attack = Random.Range(minAttack, maxAttack + 1),
            AttackSpeedRate = Random.Range(minAttackSpeedRate, maxAttackSpeedRate),
            MoveSpeedRate = Random.Range(minMoveSpeedRate, maxMoveSpeedRate),
            ShieldHp = 2*(Random.Range(minShieldHp, maxShieldHp + 1))
        };

        PlayerStartStatStorage.SetRolledStat(rolledStat);

    }
}