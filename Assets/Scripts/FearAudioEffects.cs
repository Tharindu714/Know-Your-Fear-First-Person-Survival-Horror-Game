// File: FearAudioEffects.cs
using UnityEngine;
using System.Collections;

public class FearAudioEffects : MonoBehaviour
{
    [Header("Low-Pass Muffle")]
    public AudioLowPassFilter lowPassFilter; // → drag the AudioLowPassFilter on this GameObject
    public float targetCutoff = 800f;        // final low-pass frequency
    public float muffleDuration = 2f;        // seconds to interpolate cutoff

    [Header("Heartbeat")]
    public AudioSource heartbeatSource;      // → drag a looping AudioSource with a heartbeat clip (Loop = true, PlayOnAwake = false)
    public float heartbeatDelay = 1f;        // seconds after muffle finishes before heartbeat

    [HideInInspector] public bool isInvokingMuffle = false;

    /// <summary>Call once when fear or health crosses a threshold. Lowers cutoff, then plays heartbeat.</summary>
    public void TriggerMuffleAndHeartbeat()
    {
        if (isInvokingMuffle) return;
        if (lowPassFilter == null || heartbeatSource == null) return;

        StartCoroutine(MuffleAndHeartbeatRoutine());
    }

    private IEnumerator MuffleAndHeartbeatRoutine()
    {
        isInvokingMuffle = true;
        float originalCutoff = lowPassFilter.cutoffFrequency;
        float elapsed = 0f;

        // Gradually reduce cutoffFrequency to targetCutoff
        while (elapsed < muffleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / muffleDuration);
            lowPassFilter.cutoffFrequency = Mathf.Lerp(originalCutoff, targetCutoff, t);
            yield return null;
        }
        lowPassFilter.cutoffFrequency = targetCutoff;

        // After a small delay, start the heartbeat loop
        yield return new WaitForSeconds(heartbeatDelay);
        if (!heartbeatSource.isPlaying)
            heartbeatSource.Play();
    }
}
