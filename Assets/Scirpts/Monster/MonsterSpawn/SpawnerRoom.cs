using System.Collections.Generic;
using UnityEngine;

public class SpawnerRoom : MonoBehaviour
{
    [Header("스폰 위치들")]
    [SerializeField] private Transform[] spawnPoints;

    private const int MIN_SPAWN_COUNT = 3;
    private const int MAX_SPAWN_COUNT = 7;

    // 총 몬스터 수 랜덤 범위
    private const int MIN_TOTAL_SPAWN_COUNT = 5;
    private const int MAX_TOTAL_SPAWN_COUNT = 21;

    // 살아있는 몬스터가 이 수 이하가 되면 추가 소환
    private int respawnThreshold = 3;

    private bool isRoomCleared;
    private int totalSpawnCount; // 실제로 소환된 몬스터의 수
    private int currentSpawnCount;      // 소환된 몬스터의 수

    private List<MonsterStateManager> aliveMonsters = new List<MonsterStateManager>(); // 현재 살아있는 몬스터 목록

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
        // 방이 활성화되면 아직 클리어되지 않은 상태로 변경
        MonsterManager.Instance.CheckAllMonstersDead(false);
    }

    private void Start()
    {
        // 스폰 위치가 없다면 바로 방 클리어 처리
        if (spawnPoints.Length == 0)
        {
            MonsterManager.Instance.CheckAllMonstersDead(true);
        }

        // 이번 방에서 생성될 총 몬스터 수 결정
        totalSpawnCount = Random.Range(MIN_TOTAL_SPAWN_COUNT, MAX_TOTAL_SPAWN_COUNT);

        // 첫 몬스터 생성
        SpawnMonsters();
    }
    private void Update()
    {
        // 방 상태 갱신
        UpdateRoomState();
    }

    // 방의 몬스터 상태를 갱신
    private void UpdateRoomState()
    {
        // 죽은 몬스터를 리스트에서 제거
        CheckAliveMonster();

        if (isRoomCleared)
        {
            return;
        }

        // 아직 소환할 몬스터가 남아있다면 추가 소환 시도
        if (currentSpawnCount < totalSpawnCount)
        {
            TryRespwanMonster();
        }

        // 모든 몬스터를 소환했다면 방 클리어 여부 확인
        if (currentSpawnCount >= totalSpawnCount)
        {
            CheckRoomClear();
        }
    }

    private void CheckRoomClear()
    {
        if (aliveMonsters.Count > 0)
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

        // 지정한 위치에 몬스터 배치
        monster.transform.position = spawnPos.position;

        // 살아있는 몬스터 등록
        aliveMonsters.Add(monster);

        // 총 소환 수 증가
        currentSpawnCount++;
    }

    private void CheckAliveMonster()
    {
        for (int i = aliveMonsters.Count - 1; i >= 0; i--)
        {
            if (aliveMonsters[i] == null || !aliveMonsters[i].gameObject.activeSelf)
            {
                aliveMonsters.RemoveAt(i);
            }
        }
    }
  
    // 조건을 만족하면 몬스터 추가 소환
    private void TryRespwanMonster()
    {
        if (spawnPoints.Length == 0) return;

        if (aliveMonsters.Count > respawnThreshold) return; // 살아있는 몬스터가 기존보다 많으면 종료

        int remainSpawnCount = totalSpawnCount - currentSpawnCount; // 앞으로 더 소환할 수 있는 남은수

        // 남은 수를 넘지 않도록 소환 수 결정
        int spawnCount = Mathf.Min(Random.Range(MIN_SPAWN_COUNT, MAX_SPAWN_COUNT), remainSpawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);

            SpawnMonster(spawnPoints[randomIndex]);
        }
    }
}