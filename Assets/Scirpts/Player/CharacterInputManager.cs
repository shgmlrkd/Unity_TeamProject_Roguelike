using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference attackAction;
    //무기 슬롯 필드
    [Header("Weapon Slots")]
    [SerializeField] private InputActionReference weaponSlot1Action;
    [SerializeField] private InputActionReference weaponSlot2Action;
    [SerializeField] private InputActionReference weaponSlot3Action;
    [SerializeField] private InputActionReference weaponSlot4Action;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

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
        lookAction.action.Enable();
        attackAction.action.Enable();

        weaponSlot1Action.action.Enable();
        weaponSlot2Action.action.Enable();
        weaponSlot3Action.action.Enable();
        weaponSlot4Action.action.Enable();

    }

    private void DisableInput()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        attackAction.action.Disable();

        weaponSlot1Action.action.Disable();
        weaponSlot2Action.action.Disable();
        weaponSlot3Action.action.Disable();
        weaponSlot4Action.action.Disable();

    }

    private void SubscribeInputEvents()
    {
        moveAction.action.performed += OnMovePerformed;
        moveAction.action.canceled += OnMoveCanceled;

        lookAction.action.performed += OnLookPerformed;

        attackAction.action.performed += OnAttackPerformed;

        weaponSlot1Action.action.performed += OnWeaponSlot1Performed;
        weaponSlot2Action.action.performed += OnWeaponSlot2Performed;
        weaponSlot3Action.action.performed += OnWeaponSlot3Performed;
        weaponSlot4Action.action.performed += OnWeaponSlot4Performed;
    }

    private void UnsubscribeInputEvents()
    {
        moveAction.action.performed -= OnMovePerformed;
        moveAction.action.canceled -= OnMoveCanceled;

        lookAction.action.performed -= OnLookPerformed;

        attackAction.action.performed -= OnAttackPerformed;

        weaponSlot1Action.action.performed -= OnWeaponSlot1Performed;
        weaponSlot2Action.action.performed -= OnWeaponSlot2Performed;
        weaponSlot3Action.action.performed -= OnWeaponSlot3Performed;
        weaponSlot4Action.action.performed -= OnWeaponSlot4Performed;
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
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        OnAttackPressed?.Invoke();
    }

    private void OnWeaponSlot1Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(0);
    }

    private void OnWeaponSlot2Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(1);
    }

    private void OnWeaponSlot3Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(2);
    }

    private void OnWeaponSlot4Performed(InputAction.CallbackContext context)
    {
        OnWeaponSelected?.Invoke(3);
    }
}