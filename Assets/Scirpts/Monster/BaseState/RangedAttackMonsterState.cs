using UnityEngine;

public class RangedAttackMonsterState : MonsterBase
{
    [Header("스켈레톤 투사체 프리팹")]
    [SerializeField] private SkeletonBullet skeletonBulletPrefab;
    [Header("마법사 투사체 프리팹")]
    [SerializeField] private MagicBullet magicBulletPrefab;

    [Header("발사 위치")]
    [SerializeField ] private Transform firePoint;

    [Header("투사체 속도")]
    [SerializeField] private float bulletSpeed;

    SkeletonBullet skeleton;
    MagicBullet magicBullet;
    Vector3 firePos;

    protected override void OnEnable()
    {
        base.OnEnable();
        rb.linearVelocity = Vector3.zero;
    }
    private void Update()
    {
        print($"RangedAttackMonsterState : {rb.linearVelocity}");
    }

    public void AnimEventSkeletonShootBullet() // 화살 (애니메이션 연결)
    {
        firePos = firePoint.position; // 총알이 나갈 위치

        if (monsterStateManager.CurrentState == MonsterStateEnum.Dead) // 죽은 상태라면 공격 이 불가능
        {
            return;
        }

        if(skeletonBulletPrefab ==  null)  // 프리팹이 없다면 발사 안됌
        {
            return;
            
        }

        if(monsterStateManager.Target == null) // 타겟이 없다면 발사 하지 않는다.
        {
            return;
        }

        skeleton = PoolManager.Instance.GetPool(skeletonBulletPrefab); // 스켈레톤 풀에서 총알 꺼내기

        skeleton.transform.position = firePos; // 풀에서 꺼낸 총알 firePos 위치로 이동

        Vector2 dir = monsterStateManager.Target.position - firePos; // firePos 위치에서 타겟 방향으로 계산

        // 총알 초기화
        skeleton.Init(dir, bulletSpeed, monsterStateManager.MonsterData.AttackDamage, gameObject);

    }

    public void AnimEvenMagicShootBullet() // 매직 볼 (애니메이션 연결)
    {
        firePos = firePoint.position; // 총알이 나갈 위치

        if (monsterStateManager.CurrentState == MonsterStateEnum.Dead) // 죽은 상태라면 공격 이 불가능
        {
            return;
        }

        if (magicBulletPrefab == null)  // 프리팹이 없다면 발사 안됌
        {
            return;

        }

        if (monsterStateManager.Target == null) // 타겟이 없다면 발사 하지 않는다.
        {
            return;
        }

        magicBullet = PoolManager.Instance.GetPool(magicBulletPrefab); // 마법사 풀에서 총알 꺼내기

        magicBullet.transform.position = firePos; // 풀에서 꺼낸 총알 firePos 위치로 이동


        Vector2 dir = monsterStateManager.Target.position - firePos; // firePos 위치에서 타겟 방향으로 계산

        // 총알 초기화
        magicBullet.Init(dir, bulletSpeed, monsterStateManager.MonsterData.AttackDamage, gameObject);
    }


}
