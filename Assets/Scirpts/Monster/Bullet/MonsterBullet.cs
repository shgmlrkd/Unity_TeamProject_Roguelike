using System.Collections;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    [Header("맞을 대상")]
    [SerializeField] private LayerMask targetLayer;

    [Header("유지 시간")]
    [SerializeField] private float lifeTime = 3.0f;

    private bool isReturned = true; // 중복 반환 방지용
    private int damage;

    private Rigidbody2D rb;
    private Coroutine lifeCo;
    private GameObject attacker;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 투사체 이동에 필요할 Rigidbody2D 가져오기
    }

    public void Init(Vector2 dir, float speed, int damage, GameObject attacker)
    {
        isReturned = false; // 반환 상태 해제

        this.damage = damage; // 발사 할때 받은 데미지 저장

        this.attacker = attacker; // 누가 쐈는지 저장

        if(dir == Vector2.zero) // 발사 위치랑 플레이어 위치(좌표)가 같다면 일단 왼쪽으로 발사
        {
            dir = Vector2.left;
        }

        dir.Normalize(); // 방향만 사용하기 위해서 정규화

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;   // 풀링 재사용 전 이전속도 제거
            rb.linearVelocity = dir * speed;    // 실제 투사체 이동속도 적용
        }

        // 화살 방향 맞추기
        transform.right = -dir;

        if(lifeCo != null)
        {
            StopCoroutine(lifeCo);
            lifeCo = null;
        }

        lifeCo = StartCoroutine(LifeCo());
    }

    public void Init(Vector2 dir, Vector3 pos, float speed, int damage, GameObject attacker)
    {
        isReturned = false; // 반환 상태 해제

        this.damage = damage; // 발사 할때 받은 데미지 저장

        this.attacker = attacker; // 누가 쐈는지 저장

        if (dir == Vector2.zero) // 발사 위치랑 플레이어 위치(좌표)가 같다면 일단 왼쪽으로 발사
        {
            dir = Vector2.left;
        }

        dir.Normalize(); // 방향만 사용하기 위해서 정규화

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;   // 풀링 재사용 전 이전속도 제거
            rb.linearVelocity = dir * speed;    // 실제 투사체 이동속도 적용
        }

        // 위치 맞추기
        transform.position = pos;
        // 화살 방향 맞추기
        transform.right = -dir;

        if (lifeCo != null)
        {
            StopCoroutine(lifeCo);
            lifeCo = null;
        }

        lifeCo = StartCoroutine(LifeCo());
    }

    private IEnumerator LifeCo()
    {
        yield return new WaitForSeconds(lifeTime);

        lifeCo = null; // 코루틴이 끝난 상태로 처리

        ReturnBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isReturned)
        {
            return;
        }

        if(!IsLayerMask(collision.gameObject.layer, targetLayer)) // 맞을 대상이 아니면 무시
        {
            return;
        }

        if(!collision.TryGetComponent(out IDamageable damageable)) // 데미지를 받을수 없다면 무시
        {
            return;
        }

        DamageInfoSet damageInfoSet = new DamageInfoSet();

        damageInfoSet.Damage = damage; // 데미지 저장

        damageInfoSet.Attacker = attacker; // 공격자 저장

        damageable.TakeDamage(damageInfoSet); // 데미지 전달

        ReturnBullet(); // 맞췄으면 반환
    }


    private bool IsLayerMask(int layer, LayerMask mask) // 해당 layer 가 mask안에 포함되어 있는지 확인용
    {
        return (mask.value & (1 << layer)) != 0; 
    }

    private void ReturnBullet()
    {
        // 중복 반환 방지
        if (isReturned)
        {
            return;
        }

        isReturned = true;

        if ( rb != null) // 풀로 돌아가기전에 속도 제거
        {
            rb.linearVelocity = Vector2.zero;
        }

        if(lifeCo != null) // 남아있는 코루틴 정리 제거
        {
            StopCoroutine (lifeCo);
            lifeCo = null;
        }

        PoolManager.Instance.ReturnPool(this);

    }

    private void OnDisable()
    {
        if (rb != null) // 비활성화 될때 속도 남는거 정지
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (lifeCo != null) // 비활성화 될때 코루틴 남는거 정지
        {
            StopCoroutine(lifeCo);
            lifeCo = null;
        }

        isReturned = true;
    }
}
