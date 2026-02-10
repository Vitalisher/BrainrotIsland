using System;
using UnityEngine;
using YG;

[RequireComponent(typeof(CharacterController))]
public class RobloxStyleController : MonoBehaviour
{
    public static RobloxStyleController instance;

    public Health health;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 8f;
    public float gravity = 20f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -60f;
    public float maxVerticalAngle = 60f;
    public float cameraDistance = 8f;
    public float cameraHeight = 2f;

    private CharacterController controller;
    public Animator animator;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    public LayerMask layerMask;

    public bool isCursorEnabled = true;

    public CharacterController charController;
    public Collider colider;
    public Rigidbody rb;
    
    [Header("Mobile Input")]
    public MobileInputController mobileInput;
    
    private bool jumpRequested = false; // Флаг для прыжка

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null)
        {
            GameObject camObj = new GameObject("PlayerCamera");
            cameraTransform = camObj.transform;
            camObj.AddComponent<Camera>();
        }
    }

    void Update()
    {
        if (TimerBeforeAdsYG.pouse == false)
        {
            if (health.isDead == false)
            {
                HandleMovement();
                HandleCamera();
                HandleCursorToggle();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
                CursoreState();
        }
    }

    void HandleMovement()
    {
        float h = DeviceChecker.IsMobile && mobileInput != null ? mobileInput.HorizontalInput : Input.GetAxis("Horizontal");
        float v = DeviceChecker.IsMobile && mobileInput != null ? mobileInput.VerticalInput : Input.GetAxis("Vertical");
    
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 moveDirection = (forward * v + right * h).normalized;
        bool isMoving = moveDirection.magnitude > 0.1f;
        animator.SetBool("isRunning", isMoving);
        Vector3 move = moveDirection * currentSpeed;
        
        HandleJump();

        velocity.y -= gravity * Time.deltaTime;
        move.y = velocity.y;
        controller.Move(move * Time.deltaTime);
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (controller.isGrounded)
        {
            animator.SetBool("isGrounded", true);
            velocity.y = -2f;

            // Проверяем либо клавишу, либо флаг от кнопки UI
            if (Input.GetButtonDown("Jump") || jumpRequested)
            {
                velocity.y = jumpForce;
                animator.SetTrigger("Jump");
                animator.SetBool("isGrounded", false);
                jumpRequested = false; // Сбрасываем флаг
            }
        }
        else
            animator.SetBool("isGrounded", false);
    }

    // Этот метод вызывается кнопкой UI
    public void Jump()
    {
        jumpRequested = true;
    }

    void HandleCamera()
    {
        bool shouldRotateCamera = false;

        if (Cursor.lockState == CursorLockMode.Locked)
            shouldRotateCamera = true;
        else
            shouldRotateCamera = Input.GetMouseButton(1);

        float mouseX, mouseY;
    
        if (DeviceChecker.IsMobile && mobileInput != null)
        {
            mouseX = mobileInput.MouseXInput;
            mouseY = mobileInput.MouseYInput;
            shouldRotateCamera = true;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        if (shouldRotateCamera)
        {
            horizontalRotation += mouseX;
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        }

        Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        Vector3 offset = new Vector3(0f, cameraHeight, -cameraDistance);
        Vector3 rotatedOffset = rotation * offset;
        Vector3 targetPosition = transform.position + rotatedOffset;
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * cameraHeight;
        if (Physics.Linecast(rayStart, targetPosition, out hit, layerMask.value))
            targetPosition = hit.point;

        cameraTransform.position = targetPosition;
        cameraTransform.LookAt(transform.position + Vector3.up * cameraHeight);
    }
    
    void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                CursorEnabled();
            else
                CursorDisabled();
        }
    }

    public void CursorEnabled()
    {
        if (DeviceChecker.IsMobile == false)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    public void CursorDisabled()
    {
        if (DeviceChecker.IsMobile == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void RespawnPlayer()
    {
        animator.SetTrigger("Jump");
        health.isDead = false;
        health.CurrentHealth = health.MaxHealth;
        health.UpdateText();
    }

    public void DeadAnimation() => animator.SetTrigger("Dead");

    private void OnEnable()
    {
        Health.onPlayerDied += DeadAnimation;
    }

    private void OnDisable()
    {
        Health.onPlayerDied -= DeadAnimation;
    }

    public void CursoreState()
    {
        isCursorEnabled = !isCursorEnabled;

        if (isCursorEnabled == true)
            CursorEnabled();
        else
            CursorDisabled();
    }

    public void PlayerDisabled()
    {
        charController.enabled = false;
        rb.isKinematic = false;
        colider.enabled = true;
    }

    public void PlayerEnabled()
    {
        charController.enabled = true;
        rb.isKinematic = true;
        colider.enabled = false;
    }
}