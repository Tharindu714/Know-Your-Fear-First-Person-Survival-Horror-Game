using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour
{
    public Light torchLight;
    private Color redColor = Color.red;
    private Color whiteColor = Color.white;
    private Coroutine flickerRoutine;

    public void FlickerToWhiteThenBack()
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        flickerRoutine = StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        torchLight.color = whiteColor;

        float timer = 0f;
        while (timer < 2f)
        {
            torchLight.enabled = !torchLight.enabled;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            timer += 0.1f;
        }

        torchLight.enabled = true;
        torchLight.color = redColor;
    }
}

