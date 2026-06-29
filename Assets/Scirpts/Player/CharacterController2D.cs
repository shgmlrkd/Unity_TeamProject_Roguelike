using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private bool invertVisualFlip = true;

    private Rigidbody2D rb;
    private CharacterInputManager inputManager;
    private PlayerAttackController attackController;
    //화면 기준 마우스 위치 좌표 도출용
    private Camera mainCamera;
    private CameraRig cameraRig;

    private PlayerInventory playerInventory;

    private PlayerStartStatBonus startStatBonus;

    private Vector2 lookDirection = Vector2.down;

    public Vector2 LookDirection => lookDirection;

    private Vector3 visualRootOriginalScale;

    private bool canMove = true;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraRig = mainCamera.gameObject.GetComponent<CameraRig>();

        rb = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<CharacterInputManager>();
        playerInventory = GetComponent<PlayerInventory>();
        attackController = GetComponent<PlayerAttackController>();
        startStatBonus = GetComponent<PlayerStartStatBonus>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (visualRoot == null)
        {
            if (animator != null)
            {
                visualRoot = animator.transform;
            }
            else if (spriteRenderer != null)
            {
                visualRoot = spriteRenderer.transform;
            }
        }

        if (visualRoot != null)
        {
            visualRootOriginalScale = visualRoot.localScale;
        }
    }

    private void OnEnable()
    {
        cameraRig.OnCameraMoveStarted += () => canMove = false;
        cameraRig.OnCameraMoveFinished += () => canMove = true;
    }

    private void OnDisable()
    {
        cameraRig.OnCameraMoveStarted -= () => canMove = false;
        cameraRig.OnCameraMoveFinished -= () => canMove = true;
    }

    private void Update()
    {
        UpdateLookDirection();
        UpdateVisualFlip();

        UpdateMoveAnimation();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Move();
    }

    private void Move()
    {
        if (rb == null || inputManager == null)
        {
            return;
        }

        float finalMoveSpeed = GetFinalMoveSpeed();

        rb.linearVelocity = inputManager.MoveInput * finalMoveSpeed;
    }
    private float GetFinalMoveSpeed()
    {
        float moveSpeedRate = 0f;

    if (playerInventory != null && playerInventory.CurrentBonusStat != null)
    {
        moveSpeedRate += playerInventory.CurrentBonusStat.MoveSpeedRate;
    }

    if (startStatBonus != null)
    {
        moveSpeedRate += startStatBonus.MoveSpeedRate;
    }

    return moveSpeed * Mathf.Max(0.1f, 1f + moveSpeedRate);
    }
    private void UpdateLookDirection()
    {
        if (mainCamera == null || inputManager == null)
        {
            return;
        }

        Vector3 mouseScreenPosition = inputManager.MousePosition;
        //절대 값 출력
        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z 
            - transform.position.z);

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        lookDirection = direction.normalized;
    }
    //바라보는 방향 따라 플립
    private void UpdateVisualFlip()
    {
        if (attackController != null && attackController.IsAttacking)
        {
            return;
        }
        if (visualRoot == null)
        {
            return;
        }
        if (Mathf.Abs(lookDirection.x) <= 0.01f)
        {
            return;
        }

        float directionSign = lookDirection.x > 0f ? 1f : -1f;

        if (invertVisualFlip)
        {
            directionSign *= -1f;
        }

        Vector3 scale = visualRootOriginalScale;
        scale.x = Mathf.Abs(visualRootOriginalScale.x) * directionSign;
        visualRoot.localScale = scale;
    }
    private void UpdateMoveAnimation()
    {
        if (animator == null || inputManager == null)
        {
            return;
        }

        animator.SetBool("IsMoving", inputManager.MoveInput != Vector2.zero);
    }
}