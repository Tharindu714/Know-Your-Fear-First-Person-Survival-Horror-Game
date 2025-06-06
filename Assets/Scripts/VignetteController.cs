// File: VignetteController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VignetteController : MonoBehaviour
{
    public static VignetteController Instance;

    [Header("References")]
    public Image vignetteImage;        // → drag a full-screen UI Image (with a circular black mask) here

    [Header("Settings")]
    public float fadeDuration = 1f;    // time to fade from alpha=0 → alpha=1

    private bool isActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (vignetteImage != null)
        {
            // Ensure it starts fully transparent
            Color c = vignetteImage.color;
            c.a = 0f;
            vignetteImage.color = c;
            vignetteImage.enabled = false;
        }
    }

    /// <summary>Call this to fade in the vignette (alpha 0 → 1).</summary>
public void ActivateVignette()
{
    if (vignetteImage == null || isActive) 
        return;

    isActive = true;
    vignetteImage.enabled = true;
    StartCoroutine(FadeInVignette());
}

public void DeactivateVignette()
{
    // If it’s already inactive (or the image is missing), do nothing.
    if (vignetteImage == null || !isActive) 
        return;

    // Stop any FadeIn coroutine (if it’s running)
    StopCoroutine(FadeInVignette());

    // Immediately disable the image and clear the flag
    vignetteImage.enabled = false;
    isActive = false;
}

    private IEnumerator FadeInVignette()
    {
        isActive = true;
        float elapsed = 0f;
        Color startC = vignetteImage.color;
        Color endC = vignetteImage.color;
        endC.a = 0.4f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            vignetteImage.color = Color.Lerp(startC, endC, t);
            yield return null;
        }

        vignetteImage.color = endC;
    }
}
