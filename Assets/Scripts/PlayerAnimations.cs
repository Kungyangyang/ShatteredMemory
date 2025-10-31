using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation References")]
    public Animator animator;
    
    [Header("Animation Parameters")]
    public float walkAnimationSpeed = 1f;
    public float runAnimationSpeed = 1.5f;
    public float animationSmoothTime = 0.1f;
    
    // Private variables
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private bool wasGrounded;
    private float moveInput; // Store the current input
    
    // Animation parameter hashes
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int isCrouchingHash = Animator.StringToHash("IsCrouching");
    private readonly int isSlidingHash = Animator.StringToHash("IsSliding");
    private readonly int isDashingHash = Animator.StringToHash("IsDashing");
    private readonly int jumpTriggerHash = Animator.StringToHash("Jump");
    private readonly int landTriggerHash = Animator.StringToHash("Land");
    private readonly int verticalVelocityHash = Animator.StringToHash("VerticalVelocity");

    void Start()
    {
        // Get references
        playerMovement = GetComponentInParent<PlayerMovement>();
        rb = GetComponentInParent<Rigidbody2D>();
        
        // If animator is not assigned, try to get it from this object or children
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }
        
        // Initialize wasGrounded
        wasGrounded = playerMovement != null ? playerMovement.IsGrounded : false;
    }

    void Update()
    {
        if (animator == null) return;
        if (playerMovement == null) return;
        
        // Get current input
        moveInput = Input.GetAxisRaw("Horizontal");
        
        UpdateAnimationParameters();
        HandleGroundTransitions();
    }

    void UpdateAnimationParameters()
    {
        // Get physics velocity for reference
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        float verticalVelocity = rb.linearVelocity.y;
        
        // ===== INPUT-BASED MOVEMENT DETECTION =====
        bool hasMovementInput = Mathf.Abs(moveInput) > 0.1f;
        bool isActuallyMoving = horizontalSpeed > 0.1f;
        
        // Determine if we should show movement animation
        bool shouldShowMovementAnimation = false;
        
        if (playerMovement.IsSliding || playerMovement.IsDashing)
        {
            // Always show movement during slides and dashes
            shouldShowMovementAnimation = true;
        }
        else if (hasMovementInput && isActuallyMoving && playerMovement.IsGrounded)
        {
            // Normal grounded movement with input
            shouldShowMovementAnimation = true;
        }
        else if (hasMovementInput && !playerMovement.IsGrounded && horizontalSpeed > 0.5f)
        {
            // Air movement with significant velocity
            shouldShowMovementAnimation = true;
        }
        
        // ===== SET ANIMATOR PARAMETERS =====
        
        // Speed parameter - immediate response to movement state
        if (shouldShowMovementAnimation)
        {
            // When moving, set speed based on movement type
            if (playerMovement.IsRunning && !playerMovement.IsCrouching && !playerMovement.IsSliding)
            {
                // Running
                animator.SetFloat(speedHash, runAnimationSpeed);
                animator.speed = runAnimationSpeed;
            }
            else
            {
                // Walking or other movement
                animator.SetFloat(speedHash, walkAnimationSpeed);
                animator.speed = walkAnimationSpeed;
            }
        }
        else
        {
            // Not moving - immediately set speed to 0
            animator.SetFloat(speedHash, 0f);
            animator.speed = 1f; // Reset to normal speed
        }
        
        // Other state parameters
        animator.SetBool(isRunningHash, playerMovement.IsRunning && shouldShowMovementAnimation);
        animator.SetBool(isGroundedHash, playerMovement.IsGrounded);
        animator.SetBool(isCrouchingHash, playerMovement.IsCrouching);
        animator.SetBool(isSlidingHash, playerMovement.IsSliding);
        animator.SetBool(isDashingHash, playerMovement.IsDashing);
        animator.SetFloat(verticalVelocityHash, verticalVelocity);
    }

    void HandleGroundTransitions()
    {
        bool isGrounded = playerMovement.IsGrounded;
        
        // Trigger jump animation
        if (!wasGrounded && isGrounded)
        {
            // Landed
            animator.SetTrigger(landTriggerHash);
        }
        else if (wasGrounded && !isGrounded && rb.linearVelocity.y > 0.1f)
        {
            // Jumped (only trigger if moving upward)
            animator.SetTrigger(jumpTriggerHash);
        }
        
        wasGrounded = isGrounded;
    }

    // Public method to trigger specific animations if needed
    public void TriggerDashAnimation()
    {
        if (animator != null)
        {
            // You can add a dash trigger if you want a specific dash animation
            // animator.SetTrigger("Dash");
        }
    }
}