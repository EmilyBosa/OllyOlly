using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 3f;
    public float distanceFromPlayer = 5f;   // orbit radius
    public float minYAngle = -20f;
    public float maxYAngle = 60f;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody _rb;
    private bool _isGrounded;
    private bool _jumpRequested;

    private float _yaw;     // horizontal rotation (around player)
    private float _pitch;   // vertical rotation (tilt up/down)

    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // Scene-based cursor behavior
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "GameScene")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Initialize camera angles
        _yaw = transform.eulerAngles.y;
        _pitch = 20f;
    }

    void Update()
    {
        HandleMouseOrbit();

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
            _jumpRequested = true;

        HandleMovement();
    }

    void FixedUpdate()
    {
        if (_jumpRequested && _isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool(IsJumping, true);
            _isGrounded = false;
            _jumpRequested = false;
        }

        animator.SetBool(IsGrounded, _isGrounded);
    }

    private void HandleMouseOrbit()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update orbit angles
        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minYAngle, maxYAngle);

        // Calculate rotation and position for orbit
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distanceFromPlayer);

        // Apply camera position and rotation
        cameraTransform.position = transform.position + offset + Vector3.up * 1.5f; // offset a bit above player
        cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized;

        if (movement.magnitude > 0.1f)
        {
            Vector3 moveDirection;

            if (Input.GetKey(KeyCode.LeftShift) && moveVertical > 0)
            {
                moveDirection = movement * (runSpeed * Time.deltaTime);
                _rb.MovePosition(transform.position + moveDirection);
                animator.SetBool(IsRunning, true);
                animator.SetBool(IsWalking, false);
            }
            else
            {
                moveDirection = movement * (walkSpeed * Time.deltaTime);
                _rb.MovePosition(transform.position + moveDirection);
                animator.SetBool(IsWalking, true);
                animator.SetBool(IsRunning, false);
            }

            // Make the player face the camera's forward direction (optional for third-person feel)
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camForward), 10f * Time.deltaTime);
        }
        else
        {
            animator.SetBool(IsWalking, false);
            animator.SetBool(IsRunning, false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isGrounded && collision.contacts[0].normal.y > 0.5f)
        {
            _isGrounded = true;
            animator.SetBool(IsJumping, false);
        }
    }
}