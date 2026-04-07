using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private InputManager inputs;
    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    private Vector2 moveInput;

    private void Awake()
    {
        inputs = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = inputs.MoveInput.normalized;
    }

    private void FixedUpdate()
    {
        float speed = inputs.IsSprinting ? sprintSpeed : moveSpeed;
        rb.linearVelocity = moveInput * speed;
    }
}
