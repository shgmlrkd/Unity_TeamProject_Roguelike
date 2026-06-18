using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private DamageInfoSet damageInfo;
    private bool isInitialized;

    public void Init(DamageInfoSet damageInfo)
    {
        this.damageInfo = damageInfo;
        isInitialized = true;//초기화
    }
    public void Clear()
    {
        isInitialized = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized)
        {
            return;
        }

        if (!other.TryGetComponent(out IDamageable damageable))
        {
            return;
        }
        //루트와 콜라이더 분리시 사용
        //IDamageable damageable = other.GetComponentInParent<IDamageable>();
        //if (damageable == null)
        //{
        //    return;
        //}

        if (damageable.IsDead)
        {
            return;
        }
        damageable.TakeDamage(damageInfo);
    }
}