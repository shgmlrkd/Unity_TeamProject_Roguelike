using UnityEngine;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    //플레이어와 플레이어 루트 중계용 스크립트

    [SerializeField] private PlayerAttackController attackController;
    [SerializeField] private PlayerHP playerHp;

    private void Awake()
    {
        if (attackController == null)
        {
            attackController = GetComponentInParent<PlayerAttackController>();
        }

        if (playerHp == null)
        {
            playerHp = GetComponentInParent<PlayerHP>();
        }
    }

    public void EnableAttackCollider()
    {
        attackController?.EnableAttackCollider();
    }

    public void DisableAttackCollider()
    {
        attackController?.DisableAttackCollider();
    }

    public void EndAttack()
    {
        attackController?.EndAttack();
    }

    public void DestroyPlayer()
    {
        playerHp?.DestroyPlayer();
    }
}