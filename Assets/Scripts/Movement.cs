using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 2f; // Speed of character walking
    public float runSpeed = 5f; // Speed of character running
    public float jumpForce = 5f; // Jump force
    public float mouseSensitivity = 5f; // Mouse sensitivity for turning
    public Transform cameraTransform; // Reference to the camera's transform

    public Animator animator; // Reference to the Animator component
    private Rigidbody _rb; // Reference to the Rigidbody component

    private bool _isGrounded; // Grounded status
    private bool _jumpRequested; // Jump request flag
    private float _verticalRotation = 0f; // Vertical rotation value

    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponent<Animator>();

        if (_rb == null)
            Debug.LogError("Rigidbody component is missing!");

        if (animator == null)
            Debug.LogError("Animator component is missing!");

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform; // Automatically assign the main camera if not set
    }

    void Update()
    {
        HandleMouseRotation();

        // Check if the player presses the jump button and is grounded
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _jumpRequested = true;
            Debug.Log("Jump requested");
        }

        HandleMovement();
    }

    void FixedUpdate()
    {
        // Perform the jump if requested
        if (_jumpRequested && _isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool(IsJumping, true); // Set jump animation
            _isGrounded = false; // Character is now airborne
            _jumpRequested = false; // Reset jump request
            Debug.Log("Jump executed and animation started");
        }

        // Set isGrounded parameter in the animator
        animator.SetBool(IsGrounded, _isGrounded);
    }

    private void HandleMouseRotation()
    {
        // Horizontal rotation (left and right)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up, mouseX);

        // Vertical rotation (up and down)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        _verticalRotation -= mouseY;
        _verticalRotation =
            Mathf.Clamp(_verticalRotation, 0f, 180f); // Clamp the vertical rotation between -90 and 90 degrees

        // Apply the vertical rotation to the camera
        cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // A and D keys
        float moveVertical = Input.GetAxis("Vertical"); // W and S keys

        // Calculate movement relative to player’s forward and right directions
        Vector3 movement = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized;

        if (movement.magnitude > 0.1f)
        {
            Vector3 moveDirection;

            if (Input.GetKey(KeyCode.LeftShift) && moveVertical > 0)
            {
                // Running
                moveDirection = movement * (runSpeed * Time.deltaTime);
                _rb.MovePosition(transform.position + moveDirection);

                animator.SetBool(IsRunning, true);
                animator.SetBool(IsWalking, false);
            }
            else
            {
                // Walking
                moveDirection = movement * (walkSpeed * Time.deltaTime);
                _rb.MovePosition(transform.position + moveDirection);

                animator.SetBool(IsWalking, true);
                animator.SetBool(IsRunning, false);
            }
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
            _isGrounded = true; // Set grounded immediately upon landing
            animator.SetBool(IsJumping, false); // Reset jump animation when landing
        }
    }

    
    
}