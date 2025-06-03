using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VignetteController : MonoBehaviour
{
 public static VignetteController I;

    private Image _vignetteImage;
    private bool _isFading = false;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);

        _vignetteImage = GetComponent<Image>();
        if (_vignetteImage == null)
            Debug.LogError("VignetteController requires an Image component on the same GameObject");
    }

    /// <summary>
    /// Fade in to full red vignette over duration seconds.
    /// </summary>
    public void FadeInRed(float duration = 2f)
    {
        if (_isFading || _vignetteImage == null) return;
        StartCoroutine(FadeRoutine(0f, 0.4f, duration)); 
        // 0.7 alpha gives a strong red cast; adjust as desired
    }

    private IEnumerator FadeRoutine(float fromAlpha, float toAlpha, float dur)
    {
        _isFading = true;
        float elapsed = 0f;
        Color col = _vignetteImage.color;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dur);
            col.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            _vignetteImage.color = col;
            yield return null;
        }
        col.a = toAlpha;
        _vignetteImage.color = col;
        _isFading = false;
    }
}
