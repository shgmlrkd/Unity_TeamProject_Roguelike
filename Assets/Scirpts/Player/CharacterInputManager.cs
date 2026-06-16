using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;

    [Header("Weapon Actions")]
    [SerializeField] private InputActionReference weapon1Action;
    [SerializeField] private InputActionReference weapon2Action;
    [SerializeField] private InputActionReference weapon3Action;
    [SerializeField] private InputActionReference weapon4Action;

    public Vector2 MoveInput { get; private set; }

    public event Action OnAttackPressed;
    public event Action<int> OnWeaponSelected;

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
        attackAction.action.Enable();

        weapon1Action.action.Enable();
        weapon2Action.action.Enable();
        weapon3Action.action.Enable();
        weapon4Action.action.Enable();
    }

    private void DisableInput()
    {
        moveAction.action.Disable();
        attackAction.action.Disable();

        weapon1Action.action.Disable();
        weapon2Action.action.Disable();
        weapon3Action.action.Disable();
        weapon4Action.action.Disable();
    }

    private void SubscribeInputEvents()
    {
        moveAction.action.performed += OnMovePerformed;
        moveAction.action.canceled += OnMoveCanceled;

        attackAction.action.performed += OnAttackPerformed;

        weapon1Action.action.performed += OnWeapon1Performed;
        weapon2Action.action.performed += OnWeapon2Performed;
        weapon3Action.action.performed += OnWeapon3Performed;
        weapon4Action.action.performed += OnWeapon4Performed;
    }

    private void UnsubscribeInputEvents()
    {
        moveAction.action.performed -= OnMovePerformed;
        moveAction.action.canceled -= OnMoveCanceled;

        attackAction.action.performed -= OnAttackPerformed;

        weapon1Action.action.performed -= OnWeapon1Performed;
        weapon2Action.action.performed -= OnWeapon2Performed;
        weapon3Action.action.performed -= OnWeapon3Performed;
        weapon4Action.action.performed -= OnWeapon4Performed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        OnAttackPressed?.Invoke();
    }

    private void OnWeapon1Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(0);
    }

    private void OnWeapon2Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(1);
    }

    private void OnWeapon3Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(2);
    }

    private void OnWeapon4Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(3);
    }
}