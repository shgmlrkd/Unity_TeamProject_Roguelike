using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour 
{
    protected Transform monsterTransform;
    protected MonsterStateManager monsterStateManager;
    protected Rigidbody2D rb;
    protected Collider2D monsterCollider2D;
    protected MonsterHP monsterHP;

    protected List<AStarNode> currentPath;
    protected AStarPathFinder pathFinder;
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
   
    protected void OnDrawGizmos()
    {
        if (currentPath == null) return;

        Gizmos.color = Color.purple;

        foreach (AStarNode node in currentPath)
        {
            Vector3 worldPos = pathFinder.Grid.GetWorldPosition(node);

            Gizmos.DrawSphere(worldPos, 0.3f);


        }
    }
}
