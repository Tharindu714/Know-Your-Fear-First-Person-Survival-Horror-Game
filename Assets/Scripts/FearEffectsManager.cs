using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class FearEffectsManager : MonoBehaviour
{
    [Header("Thresholds")]
    public int blurThreshold = 10;
    public int redThreshold = 12;

    [Header("Post-Processing")]
    public PostProcessVolume volume;
    public float fadeDuration = 1.5f;

    private DepthOfField _dof;
    private Vignette _vignette;
    private bool _blurTriggered = false;
    private bool _vignetteTriggered = false;

    void Start()
    {
        var profile = volume.profile;
        profile.TryGetSettings(out _dof);
        profile.TryGetSettings(out _vignette);

        volume.weight = 0f;
        _vignette.intensity.value = 0f;
        // make vignette red:
        _vignette.color.value = Color.red;
    }

    public void OnFearUpdated(int currentFearCount)
    {
        // 1) Blur in
        if (!_blurTriggered && currentFearCount >= blurThreshold)
        {
            Debug.Log("Starting blur at scare #" + currentFearCount);
            _blurTriggered = true;
            StartCoroutine(FadeVolume(1f, fadeDuration));
            // also ramp DOF parameters to max blur
            _dof.aperture.value = Mathf.Lerp(_dof.aperture.value, 32f, 1f);
            _dof.focalLength.value = Mathf.Lerp(_dof.focalLength.value, 300f, 1f);
            _dof.focusDistance.value = 0.1f;
        }

        // 2) Red vignette
        if (!_vignetteTriggered && currentFearCount >= redThreshold)
        {
            Debug.Log("Starting red vignette at scare #" + currentFearCount);
            _vignetteTriggered = true;
            if (volume.weight == 0f)
                volume.weight = 1f;
            StartCoroutine(FadeVignette(1f, fadeDuration));
        }
    }

    private IEnumerator FadeVolume(float target, float dur)
    {
        float start = volume.weight, t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            volume.weight = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        volume.weight = target;
    }

    private IEnumerator FadeVignette(float target, float dur)
    {
        float start = _vignette.intensity.value, t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        _vignette.intensity.value = target;
    }
}
