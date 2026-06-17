using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference attackAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public event Action OnAttackPressed;

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