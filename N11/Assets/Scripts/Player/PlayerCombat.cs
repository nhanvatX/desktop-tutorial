using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;
    
    [Header("Combo System")]
    public float comboWindow = 0.5f; // Thời gian để thực hiện combo tiếp theo
    public float comboCooldown = 1f; // Thời gian nghỉ sau khi hết combo
    
    [Header("Special Attack")]
    public float specialChargeTime = 2f;
    public GameObject specialProjectilePrefab;
    public Transform specialSpawnPoint;
    public float specialDamageMultiplier = 2f;
    
    private PlayerStats playerStats;
    private PlayerAnimator playerAnimator;
    private PlayerController playerController;
    
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool isChargingSpecial = false;
    private float specialChargeTimer = 0f;
    private Coroutine comboResetCoroutine;
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerController = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        HandleSpecialAttack();
    }
    
    public void Attack()
    {
        if (!canAttack || isAttacking || isChargingSpecial) return;
        
        isAttacking = true;
        
        // Reset combo reset timer
        if (comboResetCoroutine != null)
        {
            StopCoroutine(comboResetCoroutine);
        }
        
        // Play attack animation
        playerAnimator.PlayAttack(currentComboIndex);
        
        // Increment combo
        currentComboIndex++;
        if (currentComboIndex >= 8) // Max 8 combo attacks
        {
            currentComboIndex = 0;
            StartCoroutine(ComboCooldown());
        }
        else
        {
            comboResetCoroutine = StartCoroutine(ResetComboAfterDelay());
        }
    }
    
    public void StartSpecialAttack()
    {
        if (!canAttack || isAttacking) return;
        
        isChargingSpecial = true;
        specialChargeTimer = 0f;
        playerAnimator.PlaySpecialCharge();
        
        // Start charging effect (outline getting brighter)
        StartCoroutine(ChargeSpecialEffect());
    }
    
    public void ReleaseSpecialAttack()
    {
        if (!isChargingSpecial) return;
        
        isChargingSpecial = false;
        
        // Execute special attack
        playerAnimator.PlaySpecialAttack();
        
        // Create projectile after animation
        StartCoroutine(CreateSpecialProjectile());
    }
    
    void HandleSpecialAttack()
    {
        if (isChargingSpecial)
        {
            specialChargeTimer += Time.deltaTime;
            
            // Update charge effect based on timer
            float chargeProgress = Mathf.Clamp01(specialChargeTimer / specialChargeTime);
            UpdateChargeEffect(chargeProgress);
        }
    }
    
    IEnumerator ChargeSpecialEffect()
    {
        while (isChargingSpecial)
        {
            // Implement outline effect here
            // You can use a shader or sprite outline effect
            yield return null;
        }
        
        // Reset outline effect
        ResetChargeEffect();
    }
    
    void UpdateChargeEffect(float progress)
    {
        // Implement outline brightness based on progress
        // This would typically involve shader parameters or sprite effects
    }
    
    void ResetChargeEffect()
    {
        // Reset outline effect
    }
    
    IEnumerator CreateSpecialProjectile()
    {
        yield return new WaitForSeconds(0.3f); // Wait for animation timing
        
        if (specialProjectilePrefab != null && specialSpawnPoint != null)
        {
            GameObject projectile = Instantiate(specialProjectilePrefab, specialSpawnPoint.position, specialSpawnPoint.rotation);
            
            // Set projectile direction based on player facing
            Vector3 direction = playerAnimator.IsFacingRight() ? Vector3.right : Vector3.left;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f; // Adjust speed as needed
            
            // Set projectile damage
            var projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.damage = playerStats.currentDamage * specialDamageMultiplier;
            }
        }
    }
    
    IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(comboWindow);
        currentComboIndex = 0;
    }
    
    IEnumerator ComboCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(comboCooldown);
        canAttack = true;
    }
    
    // Called by Animation Event
    public void OnAttackHit()
    {
        // Detect enemies in attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            // Deal damage to enemy
            // var enemyHealth = enemy.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     float damage = playerStats.currentDamage;
            //     enemyHealth.TakeDamage(damage);
            //     
            //     // Show damage text
            //     ShowDamageText(enemy.transform.position, damage);
            //     
            //     // Gain experience
            //     playerStats.GainExp(10f); // Adjust exp gain as needed
            // }
        }
    }
    
    // Called by Animation Event
    public void OnAttackEnd()
    {
        isAttacking = false;
    }
    
    void ShowDamageText(Vector3 position, float damage)
    {
        // Implement damage text display
        // You can use a UI system or floating text prefab
        Debug.Log($"Damage: {damage}");
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}