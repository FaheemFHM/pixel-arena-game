using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private InputManager inputs;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField, Range(0f, 10f)] private float moveSpeed = 4f;
    [SerializeField, Range(0f, 10f)] private float sprintSpeed = 6f;
    [SerializeField, Range(0f, 1f)] private float verticalMoveDamping = 0.9f;
    private Vector2 moveInput;
    private Vector2 aimInput;
    private Vector2 prevAimInput;

    [Header("Shooting")]
    [SerializeField] private Transform gunPivot; // does the spin effect
    private Transform gunHolder; // does the aiming rotation
    private Transform gunTransform; // does the recoil translation

    [SerializeField, Range(0f, 360f)] private float gunSpinSpeed = 135f;
    [SerializeField] private bool gunSpinClockwise = true;

    private void Awake()
    {
        // components
        inputs = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();

        // transforms
        gunHolder = gunPivot.GetChild(0);
        gunTransform = gunHolder.GetChild(0);
    }

    private void Update()
    {
        // get inputs
        moveInput = inputs.MoveInput.normalized;
        aimInput = inputs.AimInput.normalized;

        // use move as aim if aim is none
        if (aimInput.sqrMagnitude > 0.01f) prevAimInput = aimInput;
        else if (moveInput.sqrMagnitude > 0.01f) prevAimInput = moveInput;
        aimInput = prevAimInput;

        // spin gun
        int spinDir = (gunSpinClockwise ? -1 : 1);
        gunPivot.Rotate(0, 0, gunSpinSpeed * spinDir * Time.deltaTime);

        // aim gun
        float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
        gunHolder.rotation = Quaternion.Euler(0, 0, angle);

        // gun direction
        Vector3 scale = gunTransform.localScale;
        scale.y = aimInput.x < 0 ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        gunTransform.localScale = scale;
    }

    private void FixedUpdate()
    {
        // move
        float speed = inputs.IsSprinting ? sprintSpeed : moveSpeed;
        Vector2 vel = new Vector2(moveInput.x, moveInput.y * verticalMoveDamping);
        rb.linearVelocity = vel * speed;
    }
}
