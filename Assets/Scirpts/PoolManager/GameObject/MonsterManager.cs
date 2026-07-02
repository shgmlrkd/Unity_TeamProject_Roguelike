using UnityEngine;

[System.Serializable]
public class BulletPool
{
    [SerializeField]
    private MonsterBullet monsterBullet;

    [SerializeField]
    private int bulletPoolSize;

    public MonsterBullet MonsterBullet => monsterBullet;
    public int BulletPoolSize => bulletPoolSize;
}

public class MonsterManager : ScenesSingleton<MonsterManager>
{
    [SerializeField] private MonsterStateManager[] monsterPrefabs;

    // 투사체
    [SerializeField] private BulletPool[] bullets;
  
    private int poolSize = 15;
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
        for (int i = 0; i < bullets.Length; i++)
        {
            PoolManager.Instance.PreloadPool(bullets[i].MonsterBullet, bullets[i].BulletPoolSize);
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

    public void SetAllMonstersDead(bool isAllMonsterDead)
    {
        this.isAllMonsterDead = isAllMonsterDead;
    }

    public bool CheckAllMonsterDead()
    {
        return isAllMonsterDead;
    }

    public MonsterBullet GetBossBullet()
    {
        return PoolManager.Instance.GetPool(bullets[2].MonsterBullet, bullets[2].MonsterBullet.name);
    }
}