using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    //공격받는 레이어, 공격 지속 시간
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float lifeTime = 0.15f;

    private readonly HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

    private DamageInfoSet damageInfo;
    private bool isInitialized;

    public void Init(DamageInfoSet damageInfo)
    {
        this.damageInfo = damageInfo;
        isInitialized = true;//초기화

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized)
        {
            return;
        }

        if (!IsTargetLayer(other.gameObject))
        {
            return;
        }

        if (!other.TryGetComponent(out IDamageable damageable))
        {
            return;
        }

        if (damageable.IsDead)
        {
            return;
        }

        if (damagedTargets.Contains(damageable))
        {
            return;
        }
        //중복 공격 방지
        damagedTargets.Add(damageable);
        damageable.TakeDamage(damageInfo);
    }

    private bool IsTargetLayer(GameObject target)
    {
        return (targetLayer.value & (1 << target.layer)) != 0;
    }
}