using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static UIManager I;

    [Header("UI Elements")]
    public Slider healthSlider;   // assign HealthSlider
    public Slider fearSlider;     // assign FearSlider

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    /// <summary>Call to update health (0–100).</summary>
    public void SetHealth(float value)
    {
        if (healthSlider != null)
            healthSlider.value = Mathf.Clamp(value, healthSlider.minValue, healthSlider.maxValue);
    }

    /// <summary>Call to update fear (0–100).</summary>
    public void SetFear(float value)
    {
        if (fearSlider != null)
            fearSlider.value = Mathf.Clamp(value, fearSlider.minValue, fearSlider.maxValue);
    }
}
