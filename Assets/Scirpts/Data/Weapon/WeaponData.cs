using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Roguelike/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string WeaponName;
    public Sprite WeaponIcon;

    [Header("Attack")]
    public int Damage = 1;
    public float AttackDistance = 0.7f;
    public Vector2 AttackBoxSize = new Vector2(1.0f, 1.0f);

    [Header("Drop")]
    public GameObject WeaponItemPrefab;
}