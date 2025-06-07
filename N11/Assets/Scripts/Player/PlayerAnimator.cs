using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    [Header("Animation Speeds")]
    public float idleAnimSpeed = 1f;
    public float runAnimSpeed = 1f;
    public float jumpAnimSpeed = 1f;
    public float attackAnimSpeed = 1f;
    public float deathAnimSpeed = 0.8f;
    
    private bool facingRight = true;
    
    // Animation state names
    private const string IDLE = "Idle";
    private const string RUN = "Run";
    private const string JUMP_START = "JumpStart";
    private const string JUMP_LOOP = "JumpLoop";
    private const string JUMP_END = "JumpEnd";
    private const string ATTACK = "Attack";
    private const string DEATH = "Death";
    private const string SPECIAL_CHARGE = "SpecialCharge";
    private const string SPECIAL_ATTACK = "SpecialAttack";
    
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void PlayIdle()
    {
        animator.speed = idleAnimSpeed;
        animator.Play(IDLE);
    }
    
    public void PlayRun()
    {
        animator.speed = runAnimSpeed;
        animator.Play(RUN);
    }
    
    public void PlayJumpStart()
    {
        animator.speed = jumpAnimSpeed;
        animator.Play(JUMP_START);
    }
    
    public void PlayJumpLoop()
    {
        animator.speed = jumpAnimSpeed;
        animator.Play(JUMP_LOOP);
    }
    
    public void PlayJumpEnd()
    {
        animator.speed = jumpAnimSpeed;
        animator.Play(JUMP_END);
    }
    
    public void PlayAttack(int comboIndex)
    {
        animator.speed = attackAnimSpeed;
        animator.SetInteger("ComboIndex", comboIndex);
        animator.Play(ATTACK);
    }
    
    public void PlayDeath()
    {
        animator.speed = deathAnimSpeed;
        animator.Play(DEATH);
    }
    
    public void PlaySpecialCharge()
    {
        animator.Play(SPECIAL_CHARGE);
    }
    
    public void PlaySpecialAttack()
    {
        animator.Play(SPECIAL_ATTACK);
    }
    
    public void Flip()
    {
        facingRight = !facingRight;
        Vector2 n = transform.localScale;
        transform.localScale = new Vector3(-n.x, n.y, 1);
    }
    
    public void SetFacing(bool faceRight)
    {
        if (facingRight != faceRight)
        {
            Flip();
        }
    }
    
    public bool IsFacingRight()
    {
        return facingRight;
    }
    
    // Animation Events (được gọi từ Animation Events trong Unity)
    public void OnAttackHit()
    {
        GetComponent<PlayerCombat>()?.OnAttackHit();
    }
    
    public void OnAttackEnd()
    {
        GetComponent<PlayerCombat>()?.OnAttackEnd();
    }
    
    public void OnDeathAnimationEnd()
    {
        GetComponent<PlayerController>()?.OnDeathAnimationComplete();
    }
}