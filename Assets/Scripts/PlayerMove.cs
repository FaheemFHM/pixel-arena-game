using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private InputManager inputs;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField, Range(0f, 10f)] private float moveSpeed = 4f;
    [SerializeField, Range(0f, 10f)] private float sprintSpeed = 6f;
    private Vector2 moveInput;
    private Vector2 aimInput;

    private void Awake()
    {
        inputs = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = inputs.MoveInput;
        aimInput = inputs.AimInput;
    }

    private void FixedUpdate()
    {
        float speed = inputs.IsSprinting ? sprintSpeed : moveSpeed;
        rb.linearVelocity = moveInput * speed;
    }
}
