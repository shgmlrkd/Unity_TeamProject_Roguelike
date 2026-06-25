using UnityEngine;

public class MonsterManager : ScenesSingleton<MonsterManager>
{
    [Header("프리펩")]
    [SerializeField] private MonsterStateManager[] monsterPrefabs;

    private int poolSize = 10;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            PoolManager.Instance.PreloadPool(monsterPrefabs[i], poolSize); // 몬스터 프리펩들을 미리 풀에 만들기
        }
    }

    public MonsterStateManager SpawnMonster()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            return null;
        }

        // 어떤 몬스터를 소환할지 랜덤으로 고르기
        int randomIndex = Random.Range(0, monsterPrefabs.Length);

        // 선택된 프리팹을 기준으로 풀에서 몬스터를 꺼냄
        MonsterStateManager monster = PoolManager.Instance.GetPool(monsterPrefabs[randomIndex]);

        // 실제 위치 지정은 SpawnerRoom에서 하니까 반환
        return monster;

    }


}
