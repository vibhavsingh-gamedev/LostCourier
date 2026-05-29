
// Handles all player movement — walks, sprints, jumps.
// Movement is relative to camera direction so it feels natural in 3rd person.

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -30f;       // -19.6 se -30 kri hai, kyunki player float kr rha tha
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;        // Empty child GO at player's feet
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController _cc;
    private Vector3 _verticalVelocity;
    private bool _isGrounded;
    private Camera _cam;

    public bool IsCarrying { get; set; } = false;

    private bool _jumpPressed = false;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _cam = Camera.main;
    }

    private void Update()
    {
        GroundCheck();
        Move();
        Jump();
        ApplyGravity();
    }

    private void GroundCheck()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_isGrounded && _verticalVelocity.y < 0)
            _verticalVelocity.y = -2f;  // Small negative so it stays grounded
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Get camera-relative directions (ignore Y so we don't move up/down)
        Vector3 camForward = _cam.transform.forward; camForward.y = 0; camForward.Normalize();
        Vector3 camRight = _cam.transform.right; camRight.y = 0; camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && !IsCarrying;
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        // Slow player a bit when carrying — feels more realistic
        if (IsCarrying) speed *= 0.75f;

        _cc.Move(moveDir * speed * Time.deltaTime);

        // Smoothly rotate player to face movement direction
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        // Sirf ek baar jump — hold karne par dobara nahi
        if (Input.GetButtonDown("Jump") && _isGrounded && !_jumpPressed)
        {
            _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _jumpPressed = true;
        }

        // Zameen par aane par reset
        if (_isGrounded && _verticalVelocity.y < 0)
            _jumpPressed = false;
    }

    private void ApplyGravity()
    {
        _verticalVelocity.y += gravity * Time.deltaTime;
        _cc.Move(_verticalVelocity * Time.deltaTime);
    }

    // Called from editor to visualize ground check sphere
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}