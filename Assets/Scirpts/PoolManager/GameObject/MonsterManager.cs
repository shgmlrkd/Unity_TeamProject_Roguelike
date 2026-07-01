using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : ScenesSingleton<MonsterManager>
{
    [SerializeField] private MonsterStateManager[] monsterPrefabs;

    // 투사체
    [SerializeField] private MonsterBullet[] bullet;
  
    private int poolSize = 10;
    private int bulletPoolSize = 110;
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
        for (int i = 0; i < bullet.Length; i++)
        {
            PoolManager.Instance.PreloadPool(bullet[i], bulletPoolSize);

        }
        
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