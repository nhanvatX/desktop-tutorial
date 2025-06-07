using UnityEngine;
using System;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseArmor = 10f;
    public float baseDamage = 20f;
    
    [Header("Level System")]
    public int currentLevel = 1;
    public float currentExp = 0f;
    public float expToNextLevel = 100f;
    public float expMultiplier = 1.5f; // Hệ số tăng exp cần thiết mỗi level
    
    [Header("Current Stats")]
    public float currentHealth;
    public float maxHealth;
    public float currentArmor;
    public float currentDamage;
    
    [Header("Level Bonuses")]   
    public float healthPerLevel = 20f;
    public float armorPerLevel = 5f;
    public float damagePerLevel = 10f;
    
    public event Action<int> OnLevelUp;
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnExpChanged;
    
    void Start()
    {
        CalculateStats();
        currentHealth = maxHealth;
    }
    
    void CalculateStats()
    {
        maxHealth = baseHealth + (healthPerLevel * (currentLevel - 1));
        currentArmor = baseArmor + (armorPerLevel * (currentLevel - 1));
        currentDamage = baseDamage + (damagePerLevel * (currentLevel - 1));
    }
    
    public void GainExp(float exp)
    {
        currentExp += exp;
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
        
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }
    
    void LevelUp()
    {
        currentExp -= expToNextLevel;
        currentLevel++;
        expToNextLevel *= expMultiplier;
        
        float oldMaxHealth = maxHealth;
        CalculateStats();
        
        // Heal player when level up
        currentHealth += (maxHealth - oldMaxHealth);
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        
        OnLevelUp?.Invoke(currentLevel);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
        
        Debug.Log($"Level Up! New Level: {currentLevel}");
    }
    
    public void TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(damage - currentArmor, damage * 0.1f); // Armor giảm damage nhưng tối thiểu nhận 10%
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    void Die()
    {
        GetComponent<PlayerController>().Die();
    }
}