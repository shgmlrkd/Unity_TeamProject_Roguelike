using UnityEngine;

public class MonsterManager : ScenesSingleton<MonsterManager>
{
    [Header("몬스터 데이터")]
    [SerializeField] private MonsterData[] monsterDatas;

    [Header("프리펩")]
    [SerializeField] private MonsterBase[] monsterPrefabs;

    private int poolSize = 10;

    protected override void Awake()
    {
        base.Awake();

        PoolManager.Instance.SetCreatePool();
        for (int i = 0; i < monsterDatas.Length; i++)
        {
            PoolManager.Instance.PreloadPool(monsterPrefabs[i], poolSize);
        }
    }

    public void SpawnMonster(Vector3 spawnPosition)
    {
        int randomMonsterIndex = Random.Range(0, monsterDatas.Length);

        PoolManager.Instance.GetPool(monsterPrefabs[randomMonsterIndex]);
    }

}
