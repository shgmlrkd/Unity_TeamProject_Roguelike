using UnityEngine;

public class MonsterManager : ScenesSingleton<MonsterManager>
{
    [Header("프리팹")]
    [SerializeField] private MonsterStateManager[] monsterPrefabs;


    [Header("화살 프리팹")]
    [SerializeField] private SkeletonBullet[] skeletonBulletPergabs;

    [Header("매직볼 프리팹")]
    [SerializeField] private MagicBullet[] MagicBulletPrefabs;



    private int poolSize = 10;

    private bool isAllMonsterDead = false;
    public bool IsAllMonsterDead => isAllMonsterDead;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            PoolManager.Instance.PreloadPool(monsterPrefabs[i], poolSize); // 몬스터 프리펩들을 미리 풀에 만들기
        }

        for (int i = 0; i < skeletonBulletPergabs.Length; ++i)
        {
            PoolManager.Instance.PreloadPool(skeletonBulletPergabs[i], poolSize);
        }

        for (int i = 0; i < MagicBulletPrefabs.Length; ++i)
        {
            PoolManager.Instance.PreloadPool(MagicBulletPrefabs[i], poolSize);
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
        print($"몬스터 프리팹 : {monsterPrefabs[randomIndex].GetType()}");
        // 선택된 프리팹을 기준으로 풀에서 몬스터를 꺼냄
        MonsterStateManager monster = PoolManager.Instance.GetPool(monsterPrefabs[randomIndex]);

        // 실제 위치 지정은 SpawnerRoom에서 하니까 반환
        return monster;

    }

    public void CheckAllMonstersDead(bool isAllMonsterDead)
    {
        this.isAllMonsterDead = isAllMonsterDead;
    }
}