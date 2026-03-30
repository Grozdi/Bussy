using UnityEngine;

/// <summary>
/// A simple first-person controller that uses Unity's CharacterController.
/// Features:
/// - Mouse look (yaw + pitch)
/// - WASD movement
/// - Jumping
/// - Gravity
/// - Ground check via sphere cast overlap
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera used for vertical look (pitch). If left empty, Camera.main is used.")]
    [SerializeField] private Transform playerCamera;

    [Header("Movement")]
    [Tooltip("Movement speed in units per second.")]
    [SerializeField] private float moveSpeed = 6f;

    [Tooltip("Jump height in world units.")]
    [SerializeField] private float jumpHeight = 1.6f;

    [Tooltip("Gravity acceleration (negative value).")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [Tooltip("Mouse sensitivity for look rotation.")]
    [SerializeField] private float mouseSensitivity = 200f;

    [Tooltip("Minimum vertical look angle.")]
    [SerializeField] private float minPitch = -85f;

    [Tooltip("Maximum vertical look angle.")]
    [SerializeField] private float maxPitch = 85f;

    [Header("Ground Check")]
    [Tooltip("Transform used as the center of the ground check sphere.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("Radius of the ground check sphere.")]
    [SerializeField] private float groundCheckRadius = 0.25f;

    [Tooltip("Layers considered as ground.")]
    [SerializeField] private LayerMask groundMask = ~0;

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;

    private bool IsGrounded => Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        // Optional but typical for FPS controls.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player body around Y axis (yaw).
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera around X axis (pitch), clamped to prevent flipping.
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        // Gather WASD input in local space.
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * inputX + transform.forward * inputZ).normalized;

        // Keep character grounded with a small downward force.
        if (IsGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Jump.
        if (IsGrounded && Input.GetButtonDown("Jump"))
        {
            // v = sqrt(2 * jumpHeight * -gravity)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity every frame.
        verticalVelocity += gravity * Time.deltaTime;

        // Build final velocity vector.
        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        // Helpful editor visualization for ground check sphere.
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
