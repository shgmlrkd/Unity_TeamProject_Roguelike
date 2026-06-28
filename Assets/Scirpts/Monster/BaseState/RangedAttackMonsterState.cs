using UnityEngine;

public class RangedAttackMonsterState : MonsterBase
{
    [Header("투사체 프리팹")]
    [SerializeField] private MonsterBullet bulletPrefab;

    [Header("발사 위치")]
    [SerializeField ] private Transform firePoint;

    [Header("투사체 속도")]
    [SerializeField] private float bulletSpeed;

    MonsterBullet bullet;
    Vector3 firePos;

    public void AnimEventShootBullet() // 애니메이션에 연결
    {
        if(monsterStateManager.CurrentState == MonsterStateEnum.Dead) // 죽은 상태라면 공격 이 불가능
        {
            return;
        }

        if(bulletPrefab ==  null)  // 프리팹이 없다면 발사 안됌
        {
            return;
            
        }

        if(monsterStateManager.Target == null) // 타겟이 없다면 발사 하지 않는다.
        {
            return;
        }

        firePos = firePoint.position; // 총알이 나갈 위치

        bullet = PoolManager.Instance.GetPool(bulletPrefab); // 풀에서 종알 꺼내기

        bullet.transform.position = firePos; // 풀에서 꺼낸 총알 firePos 위치로 이동

        Vector2 dir = monsterStateManager.Target.position - firePos; // firePos 위치에서 타겟 방향으로 계산


        // 총알 초기화
        bullet.Init(dir, bulletSpeed, monsterStateManager.MonsterData.AttackDamage, gameObject);

    }

}
