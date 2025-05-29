using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float intensity = 0.1f;
    public float frequency = 20f;
    public float duration = 1f;

    private Vector3 originalPos;
    private float elapsed = 0f;
    private bool shaking = false;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shaking)
        {
            if (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float x = Mathf.PerlinNoise(Time.time * frequency, 0f) * 2 - 1;
                float y = Mathf.PerlinNoise(0f, Time.time * frequency) * 2 - 1;
                transform.localPosition = originalPos + new Vector3(x, y, 0) * intensity;
            }
            else
            {
                shaking = false;
                transform.localPosition = originalPos;
            }
        }
    }

    public void TriggerShake(float dur, float inten, float freq)
    {
        duration = dur;
        intensity = inten;
        frequency = freq;
        elapsed = 0f;
        shaking = true;
    }
}

