using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSummonState : BossBase
{
    private const float TARGET_ALPHA = 0.6f;
    private const float ALPHA_DURATION = 0.95f;
    private const float SUMMON_COOLTIME = 15.0f;

    private const float SCATTER_RADIUS = 2.5f;
    
    private WaitForSeconds alphaDuration = new WaitForSeconds(ALPHA_DURATION);

    private List<GameObject> monsters = new List<GameObject>(); 

    private float coolTimer = 0.0f;

    private void Update()
    {
        coolTimer += Time.deltaTime;
    }

    public override void Enter()
    {
        if (!CanSummon())
        {
            ChangeState(BossStateEnum.Idle);
            return;
        }

        // 벽을 감지해서 오버랩이 되어있다면 소환 안함 다시 Chase로 보냄
        Collider2D wall = Physics2D.OverlapCircle(bossContext.rb.position, SCATTER_RADIUS, LayerMask.GetMask("Obstacle"));

        if (wall != null)
        {
            ChangeState(BossStateEnum.Chase);
            return;
        }

        base.Enter();
        coolTimer = 0.0f;
        bossContext.animController.OnBossSummonTrigger();
    }

    private void OnStartSummon()
    {
        StartCoroutine(SummonCoroutine());
    }

    private IEnumerator SummonCoroutine()
    {
        SoundManager.Instance.PlaySFX(SoundKey.BossSpawnMonster);

        bossContext.RequestInvincible(true);
        bossContext.RequestFade(TARGET_ALPHA, ALPHA_DURATION);
        //bossContext.OnInvincibleChanged?.Invoke(true);
        //bossContext.OnFadeRequested?.Invoke(TARGET_ALPHA, ALPHA_DURATION);

        yield return alphaDuration;

        print(bossContext.SummonMonsterCount);

        for(int i = 0; i < bossContext.SummonMonsterCount; i++)
        {
            MonsterStateManager monster = MonsterManager.Instance.SpawnMonster();

            monsters.Add(monster.gameObject);

            MonsterHP hp = monster.GetComponent<MonsterHP>();
            hp.OnMonsterDied += SummonedMonsterDeath;

            Vector3 bossPos = bossContext.rb.position;
            Vector3 offsetPos = GetRadialOffset(i, bossContext.SummonMonsterCount, SCATTER_RADIUS);
            
            Vector3 spawnPos = bossPos + offsetPos;

            monster.transform.position = spawnPos;
        }

        ChangeState(BossStateEnum.Idle);
    }

    private void SummonedMonsterDeath(GameObject monster)
    {
        monsters.Remove(monster);

        MonsterHP hp = monster.GetComponent<MonsterHP>();
        hp.OnMonsterDied -= SummonedMonsterDeath;

        if (monsters.Count == 0)
        {
            bossContext.RequestInvincible(false);
            bossContext.RequestFade(1.0f, ALPHA_DURATION);
        }
    }

    private bool CanSummon()
    {
        return coolTimer >= SUMMON_COOLTIME
            && monsters.Count == 0;
    }

    private void OnDrawGizmos()
    {
        if (bossContext == null || bossContext.rb == null)
            return;

        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(bossContext.rb.position, SCATTER_RADIUS);
    }

    public override void Tick() { }
}