using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "GameData/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string WeaponName;
    public Sprite WeaponIcon;

    [Header("Attack")]
    public GameObject AttackPrefab;
    public int Damage = 1;
    public float AttackCooldown = 0.3f;
}