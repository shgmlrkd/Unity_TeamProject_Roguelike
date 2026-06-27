using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerRoom : MonoBehaviour
{
    [Header("스폰 위치들")]
    [SerializeField] private Transform[] spawnPoints;

    private const int MIN_SPAWN_COUNT = 2;
    private const int MAX_SPAWN_COUNT = 8;

    // 총 소환수 랜덤 범위
    private const int MIN_TOTAL_SPAWN_COUNT = 3;
    private const int MAX_TOTAL_SPAWN_COUNT = 8;

    // 살아있는 몬스터가 이 수 이하가 되면 추가 소환
    private int respawnThreshold = 3;

    private bool isRoomCleared;
    private int totalSpawnCount; // 실제로 소환된 몬스터의 수
    private int currentSpawnCount;      // 소환된 몬스터의 수

    private List<MonsterStateManager> ailveMonsters = new List<MonsterStateManager>(); // 현재 살아있는 몬스터 목록

    private void Awake()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Transform[] cildren = GetComponentsInChildren<Transform>();
            List<Transform> points = new List<Transform>();

            for (int i = 0; i < cildren.Length; i++)
            {
                if (cildren[i] == transform)
                {
                    continue;
                }
                points.Add(cildren[i]);
            }
            spawnPoints = points.ToArray();
        }
    }
    private void OnEnable()
    {
        MonsterManager.Instance.CheckAllMonstersDead(false);
    }

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            MonsterManager.Instance.CheckAllMonstersDead(true);
        }

        totalSpawnCount = UnityEngine.Random.Range(MIN_TOTAL_SPAWN_COUNT, MAX_TOTAL_SPAWN_COUNT); // 몇 마리 소환될지 결정

        // 게임 시작하면 바로 몬스터 생성
        SpawnMonsters();
    }
    private void Update()
    {
        CheckAliveMonster(); // 풀로 돌아가서 비활성화 된 몬스터 리스트에서 제거

        TryRespwanMonster(); // 조건이 맞다면 추가 소환

        CheckRoomClear();
    }

    private void CheckRoomClear()
    {
        if(isRoomCleared)
        {
            return;
        }

        if (currentSpawnCount < totalSpawnCount)
        {
            return;
        }

        if (ailveMonsters.Count > 0)
        {
            return;
        }

        isRoomCleared = true;

        MonsterManager.Instance.CheckAllMonstersDead(true);
    }

    public void SpawnMonsters()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        // 처음에는 spawnPos 보다 많이 소환하지는 않음
        int spawnCount = Random.Range(MIN_SPAWN_COUNT, MAX_SPAWN_COUNT);
            //Mathf.Min(spawnPoints.Length, totalSpawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);

            SpawnMonster(spawnPoints[randomIndex]);
        }

    }

    private void SpawnMonster(Transform spawnPos)
    {
        MonsterStateManager monster = MonsterManager.Instance.SpawnMonster();

        if (monster == null)
        {
            return;
        }

        // 위치 지정은 스폰에서 한다
        monster.transform.position = spawnPos.position;

        // 살아있는 몬스터 등록
        ailveMonsters.Add(monster);

        // 지금까지 소환한 몬스터 수 증가
        currentSpawnCount++;
    }

    private void CheckAliveMonster()
    {
        for (int i = ailveMonsters.Count - 1; i >= 0; i--)
        {
            if (ailveMonsters[i] == null || !ailveMonsters[i].gameObject.activeSelf)
            {
                ailveMonsters?.Remove(ailveMonsters[i]);
            }
        }
    }
    private void TryRespwanMonster()
    {
        if (spawnPoints.Length == 0) return;

        if (isRoomCleared) return;
       
        if (currentSpawnCount >= totalSpawnCount) return; // 총 소환수를 다썼다면 종료

        if (ailveMonsters.Count > respawnThreshold) return; // 살아있는 몬스터가 기존보다 많으면 종료

        int remainSpawnCount = totalSpawnCount - currentSpawnCount; // 앞으로 더 소환할 수 있는 남은수

        int spawnCount = 0;

        if(remainSpawnCount <= MIN_SPAWN_COUNT)
        {
            spawnCount = remainSpawnCount;
        }

        else if(remainSpawnCount < MAX_SPAWN_COUNT)
        {
            spawnCount = Random.Range(remainSpawnCount, MAX_SPAWN_COUNT);
        }

        else if(remainSpawnCount == MAX_SPAWN_COUNT)
        {
            spawnCount = MAX_SPAWN_COUNT;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);

            SpawnMonster(spawnPoints[randomIndex]);
        }
    }
}
