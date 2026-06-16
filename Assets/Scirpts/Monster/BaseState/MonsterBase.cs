using UnityEngine;

public  class MonsterBase : MonoBehaviour
{
    [SerializeField] protected MonsterData monsterData;
    
    protected int currentHp;

    protected Transform target;
    protected Transform monsterTransform;
    protected MonsterStateManager monsterStateManager;

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
    }
    

}
