using System.Collections;
using UnityEngine;

public class ShadowFadeController : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeInTime = 0.5f;
    public float visibleTime = 1.0f;
    public float fadeOutTime = 1.0f;

    private Material mat;
    private Color color;

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        color = mat.color;
        color.a = 0;
        mat.color = color;
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // Fade in
        for (float t = 0f; t < fadeInTime; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0f, 1f, t / fadeInTime);
            mat.color = color;
            yield return null;
        }
        color.a = 1f; mat.color = color;

        // Stay visible
        yield return new WaitForSeconds(visibleTime);

        // Fade out
        for (float t = 0f; t < fadeOutTime; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            mat.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }
}
