using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : ScenesSingleton<MonsterManager>
{
    [SerializeField] private MonsterStateManager[] monsterPrefabs;

    // 투사체
    [Header("화살 프리팹")]
    [SerializeField] private SkeletonBullet skeletonBulletPrefab;

    [Header("매직볼 프리팹")]
    [SerializeField] private MagicBullet MagicBulletPrefab;

    private int poolSize = 10;
    private bool isAllMonsterDead = false;
    public bool IsAllMonsterDead => isAllMonsterDead;

    protected override void Awake()
    {
        base.Awake();

        PoolManager.Instance.SetCreatePool();

        //// 몬스터
        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            PoolManager.Instance.PreloadPool(monsterPrefabs[i], poolSize); 

        }
        // 투사체
        PoolManager.Instance.PreloadPool(skeletonBulletPrefab, poolSize); // 스켈레톤 화살
        PoolManager.Instance.PreloadPool(MagicBulletPrefab, poolSize); // 스켈레톤 마법사 마법볼

    }

    public MonsterStateManager SpawnMonster()
    {
        int randomIndex = Random.Range(0,4);

        return GetRandomMonster(randomIndex);

        // 실제 위치 지정은 SpawnerRoom에서 하니까 반환
    }

    private MonsterStateManager GetRandomMonster(int index)
    {
        return PoolManager.Instance.GetPool(monsterPrefabs[index], monsterPrefabs[index].name);
    }

    public void CheckAllMonstersDead(bool isAllMonsterDead)
    {
        this.isAllMonsterDead = isAllMonsterDead;
    }
}