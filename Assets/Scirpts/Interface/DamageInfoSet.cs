using UnityEngine;

public struct DamageInfoSet
{
    public int Damage;
    public GameObject Attacker;
    public Vector2 AttackDirection;

    public DamageInfoSet(
        int damage,
        GameObject attacker = null,
        Vector2 attackDirection = default)
    {
        this.Damage = damage;
        this.Attacker = attacker;
        this.AttackDirection = attackDirection;
    }
}