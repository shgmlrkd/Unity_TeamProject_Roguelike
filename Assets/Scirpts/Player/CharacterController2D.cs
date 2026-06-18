using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private CharacterInputManager inputManager;
    //화면 기준 마우스 위치 좌표 도출용
    private Camera mainCamera;

    private Vector2 lookDirection = Vector2.down;

    public Vector2 LookDirection => lookDirection;

    private void Awake()
    {
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<CharacterInputManager>();



        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

    }

    private void Update()
    {
        UpdateLookDirection();
        UpdateSpriteFlip();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.linearVelocity = inputManager.MoveInput * moveSpeed;
    }
    
    private void UpdateLookDirection()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseScreenPosition = inputManager.MousePosition;
        //절대 값 출력
        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        lookDirection = direction.normalized;
    }
    //스프라이트 플립 바라보는 방향 따라
    private void UpdateSpriteFlip()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (lookDirection.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (lookDirection.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
}