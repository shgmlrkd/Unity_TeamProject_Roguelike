using UnityEngine;

public class RangedAttackMonsterState : MonsterBase
{
    [SerializeField] private MonsterBullet monsterBulletPrefabs;

    [Header("발사 위치")]
    [SerializeField ] private Transform firePoint;

    [Header("투사체 속도")]
    [SerializeField] private float bulletSpeed;

    Vector3 firePos;

    protected override void OnEnable()
    {
        base.OnEnable();
        rb.linearVelocity = Vector3.zero;
    }

    public void AnimEventShootBullet() // 애니메이션 연결
    {
        firePos = firePoint.position; // 총알이 나갈 위치

        if (monsterStateManager.CurrentState == MonsterStateEnum.Dead) // 죽은 상태라면 공격 이 불가능
        {
            return;
        }

        if(monsterBulletPrefabs ==  null)  // 프리팹이 없다면 발사 안됌
        {
            return;
            
        }

        if(monsterStateManager.Target == null) // 타겟이 없다면 발사 하지 않는다.
        {
            return;
        }

        MonsterBullet monsterBullet;

        monsterBullet = PoolManager.Instance.GetPool(monsterBulletPrefabs, monsterBulletPrefabs.gameObject.name); // 풀에서 총알 꺼내기

        monsterBullet.transform.position = firePos; // 풀에서 꺼낸 총알 firePos 위치로 이동

        Vector2 dir = monsterStateManager.Target.position - firePos; // firePos 위치에서 타겟 방향으로 계산

        // 총알 초기화
        monsterBullet.Init(dir, bulletSpeed, monsterStateManager.MonsterData.AttackDamage, gameObject);

    }

}
