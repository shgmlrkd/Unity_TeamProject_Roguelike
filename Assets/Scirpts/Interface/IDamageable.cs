public interface IDamageable
{
    bool IsDead { get; }

    void TakeDamage(DamageInfoSet damageInfoset);
}
