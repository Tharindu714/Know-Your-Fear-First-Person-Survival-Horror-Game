using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>Call to reduce health by “amount.” Clamps at zero.</summary>
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        if (currentHealth <= 0f)
            Die(); 
    }

    /// <summary>Call to restore health by “amount.” Clamps at maxHealth.</summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    private void Die()
    {
        // Hand off to GameManager or FearManager to handle lives/loss
        FearManager.I.LoseLife();  
    }
}
