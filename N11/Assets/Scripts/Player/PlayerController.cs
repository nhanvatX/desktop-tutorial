using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask;
    
    [Header("Platform Drop")]
    public LayerMask platformLayerMask;
    
    private Rigidbody2D rb;
    private PlayerAnimator playerAnimator;
    private PlayerCombat playerCombat;
    private PlayerStats playerStats;
    
    private float horizontalInput;
    private bool isGrounded;
    private bool isJumping;
    private bool isDead;
    private bool canDropThroughPlatform;
    
    // Input states
    private bool jumpPressed;
    private bool attackPressed;
    private bool specialPressed;
    private bool specialHeld;
    private bool interactPressed;
    private bool downHeld;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerCombat = GetComponent<PlayerCombat>();
        playerStats = GetComponent<PlayerStats>();
    }
    
    void Update()
    {
        if (isDead) return;
        
        HandleInput();
        CheckGrounded();
        HandleMovement();
        HandleJump();
        HandleCombat();
        HandleAnimations();
        Debug.Log("Current Health: " + playerStats.currentHealth);
    }
    
    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetKeyDown(KeyCode.X);
        attackPressed = Input.GetKeyDown(KeyCode.Z);
        specialPressed = Input.GetKeyDown(KeyCode.S);
        specialHeld = Input.GetKey(KeyCode.S);
        interactPressed = Input.GetKeyDown(KeyCode.UpArrow);
        downHeld = Input.GetKey(KeyCode.DownArrow);
        
        // Handle special attack release
        if (Input.GetKeyUp(KeyCode.S))
        {
            playerCombat.ReleaseSpecialAttack();
        }
    }
    
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        
        if (isGrounded && isJumping)
        {
            isJumping = false;
            playerAnimator.PlayJumpEnd();
        }
    }
    
    void HandleMovement()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        
        // Flip sprite based on movement direction
        if (horizontalInput > 0)
        {
            playerAnimator.SetFacing(true);
        }
        else if (horizontalInput < 0)
        {
            playerAnimator.SetFacing(false);
        }
    }
    
    void HandleJump()
    {
        if (jumpPressed)
        {
            if (isGrounded)
            {
                if (downHeld)
                {
                    // Drop through platform
                    DropThroughPlatform();
                }
                else
                {
                    // Normal jump
                    Jump();
                }
            }
        }
    }
    
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
        playerAnimator.PlayJumpStart();
    }
    
    void DropThroughPlatform()
    {
        // Check if standing on a platform
        Collider2D platformCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayerMask);
        if (platformCollider != null)
        {
            // Temporarily ignore collision with platform
            StartCoroutine(IgnorePlatformCollision(platformCollider));
        }
    }
    
    System.Collections.IEnumerator IgnorePlatformCollision(Collider2D platform)
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platform, true);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platform, false);
    }
    
    void HandleCombat()
    {
        if (attackPressed)
        {
            playerCombat.Attack();
        }
        
        if (specialPressed)
        {
            playerCombat.StartSpecialAttack();
        }
    }
    
    void HandleAnimations()
    {
        if (isJumping && rb.linearVelocity.y > 0.1f)
        {
            // Rising
            if (!IsCurrentAnimation("JumpStart"))
                playerAnimator.PlayJumpLoop();
        }
        else if (!isGrounded)
        {
            // Falling
            playerAnimator.PlayJumpLoop();
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            // Running
            if (!IsCurrentAnimation("Run") && !IsCurrentAnimation("Attack"))
                playerAnimator.PlayRun();
        }
        else
        {
            // Idle
            if (!IsCurrentAnimation("Idle") && !IsCurrentAnimation("Attack") && !IsCurrentAnimation("SpecialCharge") && !IsCurrentAnimation("SpecialAttack"))
                playerAnimator.PlayIdle();
        }
    }
    
    bool IsCurrentAnimation(string animationName)
    {
        return playerAnimator.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
    
    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        playerAnimator.PlayDeath();
    }
    
    public void OnDeathAnimationComplete()
    {
        // Show Game Over screen
        Debug.Log("Game Over!");
        // Implement game over logic here
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}