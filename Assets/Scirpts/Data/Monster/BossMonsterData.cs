using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "BossMonsterData", menuName = "GameData/BossMonsterData")]
public class BossMonsterData : ScriptableObject
{
    [SerializeField]
    private int maxHp;

    [SerializeField]
    private int attackDamage;

    [SerializeField]
    private int projectileSpeed;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float attackSelectRange;

    [SerializeField]
    private SpriteLibraryAsset normalSpriteLibrary;

    [SerializeField]
    private SpriteLibraryAsset angrySpriteLibrary;

    public int MaxHp => maxHp;
    public int AttackDamage => attackDamage;
    public int ProjectileSpeed => projectileSpeed;
    public float MoveSpeed => moveSpeed;
    public float AttackSelectRange => attackSelectRange;
    public SpriteLibraryAsset NormalSpriteLibrary => normalSpriteLibrary;
    public SpriteLibraryAsset AngrySpriteLibrary => angrySpriteLibrary;
}