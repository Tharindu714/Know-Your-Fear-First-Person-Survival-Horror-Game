using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        UIManager.I?.SetHealth(currentHealth);  // initialize the bar
    }

    /// <summary>Call to reduce health by “amount.” Clamps at zero.</summary>
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        UIManager.I?.SetHealth(currentHealth);

        if (currentHealth <= 0f)
            Die(); 
    }

    /// <summary>Call to restore health by “amount.” Clamps at maxHealth.</summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UIManager.I?.SetHealth(currentHealth);
    }

    private void Die()
    {
        GameManager.I.LoseLife();
    }
}
