using UnityEngine;

public class MonsterBase : MonoBehaviour 
{
    protected Transform monsterTransform;
    protected MonsterStateManager monsterStateManager;
    protected Rigidbody2D rb;
    protected Collider2D monsterCollider2D;
    protected MonsterHP monsterHP;

    protected virtual void Awake()
    {

  
        if (monsterTransform == null)
        {
            monsterTransform = transform;
        }

        if (monsterStateManager == null)
        {
            monsterStateManager = GetComponent<MonsterStateManager>();
        }
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (monsterCollider2D == null) 
        {
            monsterCollider2D = GetComponent<Collider2D>();
        }
        if (monsterHP == null) 
        {
            monsterHP = GetComponent<MonsterHP>();
        }    
        
    }
   
}
