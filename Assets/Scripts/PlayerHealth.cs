// File: PlayerHealth.cs
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth = 100f;
    [HideInInspector] public float fear = 0f;

    [Header("References")]
    public FearAudioEffects fearAudio;      // → drag a GameObject that has FearAudioEffects on it

    private void Awake()
    {
        currentHealth = maxHealth;
        fear = 0f;

        // Push initial values to UI
        UIManager.I?.SetHealth(currentHealth);
        UIManager.I?.SetFear(fear);
    }

    /// <summary>Call to inflict damage. Clamps at 0, updates UI, triggers death if ≤ 0.</summary>
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        UIManager.I?.SetHealth(currentHealth);

        if (currentHealth <= 0f)
        {
            Debug.Log("Player died! (TakeDamage)");
            // You can call a GameOver method here if desired.
        }

        // Optional: if health ≤ 50 and you want breathing/heartbeat based on health:
        if (currentHealth <= 50f && !fearAudio.isInvokingMuffle)
        {
            fearAudio.TriggerMuffleAndHeartbeat();
        }
    }

    /// <summary>Call to directly decrease health (e.g. from a candle ghost). Updates UI.</summary>
    public void DecreaseHealth(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        UIManager.I?.SetHealth(currentHealth);

        if (currentHealth <= 0f)
        {
            Debug.Log("Player died! (DecreaseHealth)");
            // GameOver logic if needed.
        }

        if (currentHealth <= 50f && !fearAudio.isInvokingMuffle)
        {
            fearAudio.TriggerMuffleAndHeartbeat();
        }
    }

    /// <summary>Call to heal the player (e.g. while near a normal candle). Clamps at maxHealth, updates UI.</summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UIManager.I?.SetHealth(currentHealth);

        // You could also “un-muffle” audio if you implement that logic here.
    }

    /// <summary>Call to raise fear by amount. Clamps 0–100, updates UI, triggers audio if fear ≥ 50.</summary>
    public void IncreaseFear(float amount)
    {
        fear = Mathf.Clamp(fear + amount, 0f, 100f);
        UIManager.I?.SetFear(fear);

        if (fear >= 50f && !fearAudio.isInvokingMuffle)
        {
            fearAudio.TriggerMuffleAndHeartbeat();
        }
    }

    /// <summary>Call to reduce fear (e.g. while near an un-extinguished candle). Clamps 0–100, updates UI.</summary>
    public void DecreaseFear(float amount)
    {
        fear = Mathf.Clamp(fear - amount, 0f, 100f);
        UIManager.I?.SetFear(fear);
    }
}

