using UnityEngine;

public struct DamageInfoSet
{
    public int damage;
    public GameObject attacker;
    public Vector2 hitVector;

    public DamageInfoSet(
        int damage,
        GameObject attacker = null,
        Vector2 hitVector = default)
    {
        this.damage = damage;
        this.attacker = attacker;
        this.hitVector = hitVector;
    }
}
