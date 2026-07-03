using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterInputManager : MonoBehaviour
{
    private const int SORTING_SCALE = 100;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference attackAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition => lookAction.action.ReadValue<Vector2>();

    public event Action OnAttackPressed;

    private SortingGroup sortingGroup;

    private void Awake()
    {
        sortingGroup = GetComponentInChildren<SortingGroup>();
    }

    private void OnEnable()
    {
        EnableInput();
        SubscribeInputEvents();
    }

    private void OnDisable()
    {
        UnsubscribeInputEvents();
        DisableInput();
    }

    private void Update()
    {
        // 움직이고 있는지 체크
        bool isMoving = MoveInput.sqrMagnitude > Mathf.Epsilon;

        // 움직인다 근데 SFX 플레이 중이 아니면 플레이 한다
        if (isMoving && !SoundManager.Instance.IsPlayingSFX(SoundKey.PlayerFootStep))
        {
            SoundManager.Instance.PlaySFX(SoundKey.PlayerFootStep);
        }

        sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * SORTING_SCALE);
    }

    private void EnableInput()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        attackAction.action.Enable();
    }

    private void DisableInput()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        attackAction.action.Disable();
    }

    private void SubscribeInputEvents()
    {
        moveAction.action.performed += OnMovePerformed;
        moveAction.action.canceled += OnMoveCanceled;

        attackAction.action.performed += OnAttackPerformed;
    }

    private void UnsubscribeInputEvents()
    {
        moveAction.action.performed -= OnMovePerformed;
        moveAction.action.canceled -= OnMoveCanceled;

        attackAction.action.performed -= OnAttackPerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
    //입력없으면 멈춤
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        OnAttackPressed?.Invoke();
    }
}