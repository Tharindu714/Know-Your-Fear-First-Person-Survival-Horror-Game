using UnityEngine;
using System.Collections;

public class FearAudioEffects : MonoBehaviour
{
    [Header("Low-Pass Muffle")]
    public AudioLowPassFilter lowPass;
    public float targetCutoff = 800f;     // how muffled
    public float muffleDuration = 2f;     // seconds to muffle

    [Header("Heartbeat")]
    public AudioSource heartbeatSource;   // loop = true, playOnAwake = false
    public float heartbeatDelay = 1f;     // when to start

    public void TriggerMuffleAndHeartbeat()
    {
        StartCoroutine(FadeLowPass(lowPass.cutoffFrequency, targetCutoff, muffleDuration));
        Invoke(nameof(StartHeartbeat), heartbeatDelay);
    }

    private IEnumerator FadeLowPass(float from, float to, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            lowPass.cutoffFrequency = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        lowPass.cutoffFrequency = to;
    }

    private void StartHeartbeat()
    {
        if (heartbeatSource != null && !heartbeatSource.isPlaying)
            heartbeatSource.Play();
    }
}

