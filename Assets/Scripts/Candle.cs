using UnityEngine;
using System;

public class Candle : MonoBehaviour
{
    public float fearReduce = 30f;
    public float healthRestore = 30f;
    public AudioClip litSound;
    public static event Action OnCandleLit;

    private bool isLit = false;
    private ParticleSystem flame;

    void Start()
    {
        flame = GetComponentInChildren<ParticleSystem>();
        flame.Stop();
    }

    public void LightCandle()
    {
        if (isLit) return;
        isLit = true;
        flame.Play();
        AudioSource.PlayClipAtPoint(litSound, transform.position);

        // Reduce fear and restore health
        // FearManager.I.ReduceFear(fearReduce);
        FindObjectOfType<PlayerHealth>()?.Heal(healthRestore);

        // Fire the candle-lit event so CandleGhost and Monster know
        OnCandleLit?.Invoke();
    }
}
