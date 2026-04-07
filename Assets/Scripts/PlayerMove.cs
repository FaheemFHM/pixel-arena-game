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
    private Vector2 moveNorm;
    private Vector2 aimInput;
    private Vector2 aimNorm;
    private Vector2 myNorm = Vector2.right;

    private int dir = 1;

    [Header("Shooting")]
    [SerializeField] private Transform gunPivot; // does the spin effect
    [SerializeField] private Transform gunHolder; // does the aiming rotation
    private Transform gunTransform; // does the recoil translation
    private Vector2 gunOffset;

    [SerializeField, Range(0f, 360f)] private float gunSpinSpeed = 135f;
    [SerializeField] private bool gunSpinClockwise = true;

    [SerializeField] private Transform crosshair;
    private SpriteRenderer crossRend;

    [SerializeField] private float crosshairDistMin = 0.5f;
    [SerializeField] private float crosshairDistMax = 3f;

    [SerializeField] private Transform sprite;

    private void Awake()
    {
        // components
        inputs = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();

        gunTransform = gunHolder.GetChild(0);
        gunOffset = Vector2.up * gunHolder.localPosition.y;

        crossRend = crosshair.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // get inputs
        moveInput = inputs.MoveInput;
        aimInput = inputs.AimInput;

        // get normalised inputs
        moveNorm = moveInput.normalized;
        aimNorm = aimInput.normalized;

        // player direction
        Vector2 myInput = aimInput.magnitude < 0.01f ? moveInput : aimInput;
        if (myInput.sqrMagnitude > 0.01f) myNorm = myInput.normalized;

        if (myNorm.x < -0.01f) dir = -1;
        else if (myNorm.x > 0.01f) dir = 1;

        // flip character
        sprite.localScale = new Vector3(dir, 1, 1);

        // spin gun
        int spinDir = (gunSpinClockwise ? -1 : 1);
        gunPivot.Rotate(0, 0, gunSpinSpeed * spinDir * Time.deltaTime);

        // aim gun
        float angle = Mathf.Atan2(myNorm.y, myNorm.x) * Mathf.Rad2Deg;
        gunHolder.rotation = Quaternion.Euler(0, 0, angle);

        // flip gun
        gunHolder.localScale = new Vector3(1, dir, 1);

        // crosshair position
        float dist = myInput.magnitude * crosshairDistMax;
        crosshair.localPosition = (myNorm * dist) + gunOffset;

        // crosshair effects
        float strength = Mathf.Clamp01(myInput.magnitude);
        float cutoff = crosshairDistMin / crosshairDistMax;
        float op = Mathf.InverseLerp(0f, cutoff, strength);

        Color c = crossRend.color;
        c.a = op;
        crossRend.color = c;

        crosshair.localScale = Vector2.one * op;
    }

    private void FixedUpdate()
    {
        // move
        float speed = inputs.IsSprinting ? sprintSpeed : moveSpeed;
        Vector2 vel = new Vector2(moveNorm.x, moveNorm.y * verticalMoveDamping);
        rb.linearVelocity = vel * speed;
    }
}
