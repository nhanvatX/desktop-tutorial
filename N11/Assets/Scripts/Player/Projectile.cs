using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 50f;
    public float lifetime = 3f;
    public float speed = 10f;
    public LayerMask enemyLayers;
    
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Destroy projectile after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit enemy
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            // var enemyHealth = other.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     enemyHealth.TakeDamage(damage);
            //     
            //     // Create hit effect
            //     CreateHitEffect();
            //     
            //     // Destroy projectile
            //     Destroy(gameObject);
            // }
        }
        
        // Destroy on collision with ground
        if (other.CompareTag("Ground"))
        {
            CreateHitEffect();
            Destroy(gameObject);
        }
    }
    
    void CreateHitEffect()
    {
        // Implement hit effect here
        // You can instantiate particle effects or other visual feedback
    }
}