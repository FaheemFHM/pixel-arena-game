using UnityEngine;
using System;
using UnityEngine.InputSystem;

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
    public bool IsMoving { get; private set; }
    public bool IsAiming { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsSecondary { get; private set; }
    public bool IsInteracting { get; private set; }
    public bool IsSwitching { get; private set; }

    // values
    public Vector2 MoveInput { get; private set; }
    public Vector2 AimInput { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        // move
        controls.Player.Move.started += ctx => SetMove(true);
        controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx =>
        {
            MoveInput = Vector2.zero;
            SetMove(false);
        };

        // aim
        controls.Player.Aim.started += ctx => SetAim(true);
        controls.Player.Aim.performed += ctx => AimInput = ctx.ReadValue<Vector2>();
        controls.Player.Aim.canceled += ctx =>
        {
            AimInput = Vector2.zero;
            SetAim(false);
        };

        // sprint
        controls.Player.Sprint.started += ctx => SetSprint(true);
        controls.Player.Sprint.canceled += ctx => SetSprint(false);

        // primary
        controls.Player.Primary.started += ctx => SetPrimary(true);
        controls.Player.Primary.canceled += ctx => SetPrimary(false);

        // secondary
        controls.Player.Secondary.started += ctx => SetSecondary(true);
        controls.Player.Secondary.canceled += ctx => SetSecondary(false);

        // interact
        controls.Player.Interact.started += ctx => SetInteract(true);
        controls.Player.Interact.canceled += ctx => SetInteract(false);

        // switch
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
        IsMoving = value;
        onMove?.Invoke(value);
    }

    private void SetAim(bool value)
    {
        IsAiming = value;
        onAim?.Invoke(value);
    }

    private void SetSprint(bool value)
    {
        IsSprinting = value;
        onSprint?.Invoke(value);
    }

    private void SetPrimary(bool value)
    {
        IsPrimary = value;
        onPrimary?.Invoke(value);
    }

    private void SetSecondary(bool value)
    {
        IsSecondary = value;
        onSecondary?.Invoke(value);
    }

    private void SetInteract(bool value)
    {
        IsInteracting = value;
        onInteract?.Invoke(value);
    }

    private void SetSwitch(bool value)
    {
        IsSwitching = value;
        onSwitch?.Invoke(value);
    }
}
