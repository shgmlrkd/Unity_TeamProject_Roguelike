using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterStateBese : MonoBehaviour
{
    protected Transform monsterTransform;
    protected MonsterStateManager monsterStateManager;
    
    protected virtual void OnEnable()
    {
        if (monsterTransform == null)
        {
            monsterTransform = transform;
        }

        if(monsterStateManager == null)
        {
            monsterStateManager = GetComponent<MonsterStateManager>();
        }

    }
    
}
