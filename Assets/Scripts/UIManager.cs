// File: UIManager.cs
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager I;

    [Header("UI Sliders (0–1 range)")]
    public Slider healthSlider;    // → drag your Health Slider here; set Max = 1 in Inspector
    public Slider fearSlider;      // → drag your Fear  Slider here; set Max = 1 in Inspector

    [Header("Player Reference")]
    public PlayerHealth playerHealth;  // → drag your Player (with PlayerHealth) here

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("UIManager: PlayerHealth reference missing!");
            return;
        }

        // Initialize both bars at the very start
        UpdateHealthUI();
        UpdateFearUI();
    }

    void Update()
    {
        // Polling each frame ensures the UI stays in sync if values change spontaneously
        UpdateHealthUI();
        UpdateFearUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null || healthSlider == null) return;

        float hp = Mathf.Clamp(playerHealth.currentHealth, 0f, playerHealth.maxHealth);
        // Slider’s max = 1, so normalize:
        healthSlider.value = hp / playerHealth.maxHealth;
    }

    private void UpdateFearUI()
    {
        if (playerHealth == null || fearSlider == null) return;

        float fr = Mathf.Clamp(playerHealth.fear, 0f, 100f);
        // Slider’s max = 1, so normalize:
        fearSlider.value = fr / 100f;
    }

    /// <summary>
    /// Call to set Health bar directly (value between 0–maxHealth).
    /// </summary>
    public void SetHealth(float value)
    {
        if (healthSlider == null || playerHealth == null) return;
        float clamped = Mathf.Clamp(value, 0f, playerHealth.maxHealth);
        healthSlider.value = clamped / playerHealth.maxHealth;
    }

    /// <summary>
    /// Call to set Fear bar directly (value between 0–100).
    /// </summary>
    public void SetFear(float value)
    {
        if (fearSlider == null) return;
        float clamped = Mathf.Clamp(value, 0f, 100f);
        fearSlider.value = clamped / 100f;
    }
}
