using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponData", menuName = "GameData/WeaponData")]
public class WeaponData : EquipmentData
{
    [Header("Weapon Attack")]
    [FormerlySerializedAs("AttackBoxSize")]
    [SerializeField] private Vector2 attackBoxSize = new Vector2(1.0f, 1.0f);

    [Header("Drop")]
    [FormerlySerializedAs("WeaponItemPrefab")]
    [SerializeField] private GameObject weaponItemPrefab;

    public Vector2 AttackBoxSize => attackBoxSize;
    public GameObject WeaponItemPrefab => weaponItemPrefab;

    //기존 플레이어 코드와 연결 유지용
    public string WeaponName => ItemName;
    public Sprite WeaponIcon => Sprite;
    public int Damage => Attack;
    public float AttackDistance => AttackRange;
}