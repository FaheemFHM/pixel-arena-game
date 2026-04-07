using UnityEngine;
using System;
using UnityEngine.InputSystem; // Make sure the new Input System package is installed

public class InputManager : MonoBehaviour
{
    private PlayerControls controls;

    // events
    public event Action<bool> onMove;
    public event Action<bool> onAim;
    public event Action<bool> onSprint;

    public event Action<bool> onPrimary;
    public event Action<bool> onSecondary;

    public event Action<bool> onInteract;
    public event Action<bool> onSwitch;

    // states
    public bool isMoving;
    public bool isAiming;
    public bool isSprinting;

    public bool isPrimary;
    public bool isSecondary;

    public bool isInteracting;
    public bool isSwitching;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        // subscriptions
        controls.Player.Move.started += ctx => SetMove(true);
        controls.Player.Move.canceled += ctx => SetMove(false);

        controls.Player.Aim.started += ctx => SetAim(true);
        controls.Player.Aim.canceled += ctx => SetAim(false);

        controls.Player.Sprint.started += ctx => SetSprint(true);
        controls.Player.Sprint.canceled += ctx => SetSprint(false);

        controls.Player.Primary.started += ctx => SetPrimary(true);
        controls.Player.Primary.canceled += ctx => SetPrimary(false);

        controls.Player.Secondary.started += ctx => SetSecondary(true);
        controls.Player.Secondary.canceled += ctx => SetSecondary(false);

        controls.Player.Interact.started += ctx => SetInteract(true);
        controls.Player.Interact.canceled += ctx => SetInteract(false);

        controls.Player.Switch.started += ctx => SetSwitch(true);
        controls.Player.Switch.canceled += ctx => SetSwitch(false);
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    // setters
    private void SetMove(bool value)
    {
        isMoving = value;
        onMove?.Invoke(value);
    }

    private void SetAim(bool value)
    {
        isAiming = value;
        onAim?.Invoke(value);
    }

    private void SetSprint(bool value)
    {
        isSprinting = value;
        onSprint?.Invoke(value);
    }

    private void SetPrimary(bool value)
    {
        isPrimary = value;
        onPrimary?.Invoke(value);
    }

    private void SetSecondary(bool value)
    {
        isSecondary = value;
        onSecondary?.Invoke(value);
    }

    private void SetInteract(bool value)
    {
        isInteracting = value;
        onInteract?.Invoke(value);
    }

    private void SetSwitch(bool value)
    {
        isSwitching = value;
        onSwitch?.Invoke(value);
    }
}
