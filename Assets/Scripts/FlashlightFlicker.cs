using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightFlicker : MonoBehaviour
{
    public Light flashlight;
    public float minTime = 0.05f;
    public float maxTime = 0.2f;

    private Coroutine flickerCoroutine;

    public void StartFlickering()
    {
        if (flickerCoroutine == null)
            flickerCoroutine = StartCoroutine(Flicker());
    }

    public void StopFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        // Ensure flashlight is OFF cleanly
        flashlight.enabled = false;
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            flashlight.enabled = !flashlight.enabled;
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}

