using UnityEngine;
using Unity.Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float sprintMultiplier = 2.5f;
    public float jumpForce = 8f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    // Crouch & Slide
    public float crouchSpeedMultiplier = 0.5f;
    public float slideDecay = 5f; // how fast slide slows down
    private bool isCrouching = false;
    private bool isSliding = false;
    private float currentSpeed;

    // Dash variables
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 10f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    public float dashCooldownTimer = 0f;
    private Vector2 dashDirection; // Now a Vector2 for directional dashing

    // Aiming variables
    private Vector2 mousePosition;
    private Camera mainCamera;

    // Crouch collider variables
    private CapsuleCollider2D playerCollider;
    private float originalColliderHeight;
    private Vector2 originalColliderOffset;
    public float crouchColliderHeight = 0.5f; // Adjust this value as needed
    
    // Crouch blocking detection
    public Transform ceilingCheck;
    public float ceilingCheckRadius = 0.2f;
    private bool isBlockedAbove = false;
    private bool wantsToStandUp = false;

    // Crouch fast fall variables
    public float fastFallMultiplier = 3f; // How much faster you fall when crouching
    public float maxFastFallSpeed = 20f; // Maximum falling speed
    public float upwardMomentumCancelSpeed = 15f; // How quickly upward momentum is canceled

    // Sound variables
    public AudioClip dashSound;
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip slideSound;
    private AudioSource audioSource;

    // Sound control variables - track current playing state
    private bool isWalkingSoundPlaying = false;
    private bool isRunningSoundPlaying = false;
    private bool isSlidingSoundPlaying = false;
    private AudioSource walkAudioSource;
    private AudioSource runAudioSource;
    private AudioSource slideAudioSource;

    //Camera run zoom out
    private CinemachineCamera vcam;
    private float defaultZoom = 5f;
    public float runZoomOut = 6.5f;
    public float zoomSmoothSpeed = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = moveSpeed;
        mainCamera = Camera.main; // Get the main camera
        
        // Get the collider and store original values
        playerCollider = GetComponent<CapsuleCollider2D>();
        if (playerCollider != null)
        {
            originalColliderHeight = playerCollider.size.y;
            originalColliderOffset = playerCollider.offset;
        }

        // Get or add AudioSource components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Create separate AudioSources for looping sounds
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        runAudioSource = gameObject.AddComponent<AudioSource>();
        slideAudioSource = gameObject.AddComponent<AudioSource>();

        // Configure the audio sources for movement sounds
        ConfigureMovementAudioSource(walkAudioSource);
        ConfigureMovementAudioSource(runAudioSource);
        ConfigureMovementAudioSource(slideAudioSource);

        vcam = FindObjectOfType<CinemachineCamera>();
        if (vcam != null)
        {
            defaultZoom = vcam.Lens.OrthographicSize;
        }
    }

    void ConfigureMovementAudioSource(AudioSource source)
    {
        source.loop = true;
        source.playOnAwake = false;
        source.volume = audioSource.volume;
        source.pitch = audioSource.pitch;
        source.spatialBlend = audioSource.spatialBlend;
    }

    void Update()
    {
        // Handle cooldowns
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Get mouse position in world coordinates
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Check if player is blocked above (only when crouching)
        if (isCrouching || isSliding)
        {
            CheckCeilingBlock();
        }
        else
        {
            isBlockedAbove = false;
        }

        // Dash input (Space key)
        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0 && !isDashing)
        {
            StartDash();
        }

        // Handle dash movement
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            // Continue dashing in the set direction
            rb.linearVelocity = dashDirection * dashSpeed;

            if (dashTimer <= 0)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
                // COMPLETELY reset velocity after dash (both horizontal and vertical)
                rb.linearVelocity = Vector2.zero;
            }

            // Skip other movement inputs during dash
            return;
        }

        // Horizontal movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        float targetSpeed = moveSpeed;

        // Update player facing direction based on mouse position
        UpdateAimDirection();

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isSliding)
        {
            targetSpeed *= sprintMultiplier;
        }
        
        bool slideInput = Input.GetMouseButton(1); // Right Mouse
        bool crouchInput = Input.GetKey(KeyCode.S); // S Key
        
        // If player wants to stop crouching but is blocked above
        if ((isCrouching || isSliding) && !crouchInput && !slideInput && isBlockedAbove)
        {
            wantsToStandUp = true;
            // Keep crouching until not blocked
            ForceCrouch();
        }
        // SLIDE INPUT (Right Mouse + Shift) - maintains momentum
        else if (slideInput && Input.GetKey(KeyCode.LeftShift))
        {
            wantsToStandUp = false;
            
            if (!isSliding)
            {
                // Start sliding
                StartSlide(moveInput);
            }
            else
            {
                // Continue sliding - apply decay to horizontal velocity (works in air too!)
                if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
                {
                    float newXVelocity = Mathf.MoveTowards(rb.linearVelocity.x, moveInput * moveSpeed, slideDecay * Time.deltaTime);
                    rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);
                }
            }
        }
        // Right Mouse without Shift = normal crouch with slowdown
        else if (slideInput && !Input.GetKey(KeyCode.LeftShift))
        {
            wantsToStandUp = false;
            isCrouching = true;
            targetSpeed *= crouchSpeedMultiplier;
            SetCrouchCollider(); // Set crouch collider for crouch
            
            // Stop sliding if we were sliding
            if (isSliding)
            {
                StopSliding();
            }
        }
        // S KEY INPUT - Fast fall only (NO horizontal slowdown)
        else if (crouchInput && !isSliding && !isDashing)
        {
            wantsToStandUp = false;
            
            // Apply fast fall when in air (regardless of current vertical velocity)
            if (!isGrounded)
            {
                // If we're moving upward, rapidly cancel upward momentum
                if (rb.linearVelocity.y > 0)
                {
                    // Rapidly reduce upward velocity to enter falling state faster
                    float newYVelocity = Mathf.MoveTowards(rb.linearVelocity.y, -maxFastFallSpeed, upwardMomentumCancelSpeed * Time.deltaTime);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, newYVelocity);
                }
                // If already falling, apply additional downward force
                else if (rb.linearVelocity.y < 0)
                {
                    rb.linearVelocity += Vector2.down * fastFallMultiplier * Time.deltaTime;
                    
                    // Clamp the maximum fall speed to prevent excessive velocity
                    if (rb.linearVelocity.y < -maxFastFallSpeed)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFastFallSpeed);
                    }
                }
                // If vertical velocity is near zero, start falling immediately
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallMultiplier);
                }
            }
            else if (isGrounded)
            {
                // On ground: just crouch normally with slowdown
                isCrouching = true;
                targetSpeed *= crouchSpeedMultiplier;
                SetCrouchCollider();
                
                // Stop sliding if we were sliding
                if (isSliding)
                {
                    StopSliding();
                }
            }
        }
        else
        {
            // Stop sliding if slide input is released
            if (isSliding)
            {
                StopSliding();
            }
            
            // Only allow standing up if not blocked above
            if (!isBlockedAbove)
            {
                isCrouching = false;
                wantsToStandUp = false;
                ResetCollider();
            }
            else
            {
                // Still blocked, force crouch
                ForceCrouch();
            }
        }

        // Apply movement - DON'T override slide velocity or dash velocity
        if (!isSliding && !isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * targetSpeed, rb.linearVelocity.y);
        }

        // Jumping (W key) - Single jump only
        if (Input.GetKeyDown(KeyCode.W) && isGrounded && !isBlockedAbove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            PlayJumpSound();
        }

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isSliding;
        
        UpdateMovementSounds();
        UpdateCameraZoom(isSprinting);
    }

    void StopSliding()
    {
        isSliding = false;
        // After slide ends, check if we can stand up or need to stay crouched
        if (isBlockedAbove)
        {
            ForceCrouch();
        }
        else
        {
            ResetCollider();
        }
    }

    void UpdateCameraZoom(bool running)
    {
        if (vcam == null) return;

        float targetZoom = running ? runZoomOut : defaultZoom;
        vcam.Lens.OrthographicSize = Mathf.Lerp(
            vcam.Lens.OrthographicSize,
            targetZoom,
            Time.deltaTime * zoomSmoothSpeed
        );
    }

    void UpdateMovementSounds()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        bool isMoving = Mathf.Abs(moveInput) > 0.1f && isGrounded;

        // Walking sound (only when grounded and moving, not running or sliding)
        bool shouldPlayWalk = isMoving && !Input.GetKey(KeyCode.LeftShift) && !isSliding && !isCrouching && isGrounded;
        if (shouldPlayWalk && !isWalkingSoundPlaying)
        {
            PlayWalkSound();
        }
        else if (!shouldPlayWalk && isWalkingSoundPlaying)
        {
            StopWalkSound();
        }

        // Running sound (only when grounded, moving, and sprinting)
        bool shouldPlayRun = isMoving && Input.GetKey(KeyCode.LeftShift) && !isSliding && !isCrouching && isGrounded;
        if (shouldPlayRun && !isRunningSoundPlaying)
        {
            PlayRunSound();
        }
        else if (!shouldPlayRun && isRunningSoundPlaying)
        {
            StopRunSound();
        }

        // Slide sound (only when grounded and sliding)
        bool shouldPlaySlide = isSliding && isGrounded;
        if (shouldPlaySlide && !isSlidingSoundPlaying)
        {
            PlaySlideSound();
        }
        else if (!shouldPlaySlide && isSlidingSoundPlaying)
        {
            StopSlideSound();
        }

        // Stop all movement sounds when airborne
        if (!isGrounded)
        {
            if (isWalkingSoundPlaying) StopWalkSound();
            if (isRunningSoundPlaying) StopRunSound();
            if (isSlidingSoundPlaying) StopSlideSound();
        }
    }

    void CheckCeilingBlock()
    {
        if (ceilingCheck != null)
        {
            // Check for obstacles above that would prevent standing up
            isBlockedAbove = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, groundLayer);
        }
    }

    void ForceCrouch()
    {
        isCrouching = true;
        SetCrouchCollider();
    }

    void UpdateAimDirection()
    {
        // Calculate direction from player to mouse
        Vector2 lookDir = mousePosition - (Vector2)transform.position;
        
        // Flip player based on mouse position
        if (lookDir.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (lookDir.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void StartSlide(float moveInput)
    {
        isSliding = true;
        float slideSpeed = moveSpeed * sprintMultiplier;
        
        // If we're moving, use slide speed in our movement direction
        if (moveInput != 0)
        {
            rb.linearVelocity = new Vector2(moveInput * slideSpeed, rb.linearVelocity.y);
        }
        // If we're not moving but sliding, maintain current horizontal momentum but boost it
        else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            float currentDirection = Mathf.Sign(rb.linearVelocity.x);
            rb.linearVelocity = new Vector2(currentDirection * slideSpeed, rb.linearVelocity.y);
        }
        // If completely stationary, slide in facing direction
        else
        {
            float facingDirection = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(facingDirection * slideSpeed, rb.linearVelocity.y);
        }
        
        SetCrouchCollider(); // Ensure collider is set for slide
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;

        // Calculate direction to mouse cursor
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mouseWorldPos - (Vector2)transform.position).normalized;

        // Set dash direction towards the mouse
        dashDirection = directionToMouse;
        
        // Stop any movement sounds during dash
        StopWalkSound();
        StopRunSound();
        StopSlideSound();
        
        // Play dash sound effect
        PlayDashSound();
    }

    // Sound Methods
    void PlayDashSound()
    {
        if (dashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }

    void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    void PlayWalkSound()
    {
        if (walkSound != null && walkAudioSource != null && !isWalkingSoundPlaying)
        {
            walkAudioSource.clip = walkSound;
            walkAudioSource.Play();
            isWalkingSoundPlaying = true;
        }
    }

    void StopWalkSound()
    {
        if (walkAudioSource != null && isWalkingSoundPlaying)
        {
            walkAudioSource.Stop();
            isWalkingSoundPlaying = false;
        }
    }

    void PlayRunSound()
    {
        if (runSound != null && runAudioSource != null && !isRunningSoundPlaying)
        {
            runAudioSource.clip = runSound;
            runAudioSource.Play();
            isRunningSoundPlaying = true;
        }
    }

    void StopRunSound()
    {
        if (runAudioSource != null && isRunningSoundPlaying)
        {
            runAudioSource.Stop();
            isRunningSoundPlaying = false;
        }
    }

    void PlaySlideSound()
    {
        if (slideSound != null && slideAudioSource != null && !isSlidingSoundPlaying)
        {
            slideAudioSource.clip = slideSound;
            slideAudioSource.Play();
            isSlidingSoundPlaying = true;
        }
    }

    void StopSlideSound()
    {
        if (slideAudioSource != null && isSlidingSoundPlaying)
        {
            slideAudioSource.Stop();
            isSlidingSoundPlaying = false;
        }
    }

    void SetCrouchCollider()
    {
        if (playerCollider != null)
        {
            // Reduce collider height and adjust offset to keep feet at the same position
            playerCollider.size = new Vector2(playerCollider.size.x, crouchColliderHeight);
            
            // Calculate new offset to keep the bottom of the collider at the same position
            float heightDifference = originalColliderHeight - crouchColliderHeight;
            playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - heightDifference * 0.5f);
        }
    }

    void ResetCollider()
    {
        if (playerCollider != null)
        {
            // Restore original collider size and offset
            playerCollider.size = new Vector2(playerCollider.size.x, originalColliderHeight);
            playerCollider.offset = originalColliderOffset;
        }
    }

    // Optional: Visual debugging in the scene view
    void OnDrawGizmosSelected()
    {
        if (playerCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (Vector3)playerCollider.offset, playerCollider.size);
        }
        
        // Draw ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // Draw ceiling check
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
        }
    }

    // Public properties for animation script access
public bool IsGrounded => isGrounded;
public bool IsRunning => Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isSliding && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;
public bool IsCrouching => isCrouching;
public bool IsSliding => isSliding;
public bool IsDashing => isDashing;
public float CurrentSpeed => Mathf.Abs(rb.linearVelocity.x);
}