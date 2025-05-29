using UnityEngine;
using System.Collections;

public class TorchEffects : MonoBehaviour
{
    [Header("Red Tint")]
    public Light torchLight;              // your flashlight’s Light component
    public Color normalColor = Color.white;
    public Color panicColor = new Color32(0xF3, 0x00, 0x00, 0xFF);
    public float flashFadeDuration = 1f;  // fade time for intensity

    private float originalIntensity;

    void Start()
    {
        if (torchLight == null)
            torchLight = GetComponent<Light>();
        originalIntensity = torchLight.intensity;
    }

    /// <summary>
    /// Call this to tint the torch red and fade its intensity.
    /// </summary>
    public IEnumerator PanicTint()
    {
        // 1) change color
        torchLight.color = panicColor;

        // 2) fade intensity down to half over duration
        float t = 0f;
        while (t < flashFadeDuration)
        {
            t += Time.deltaTime;
            torchLight.intensity = Mathf.Lerp(originalIntensity, originalIntensity * 0.5f, t / flashFadeDuration);
            yield return null;
        }
        torchLight.intensity = originalIntensity * 0.5f;
    }
}


