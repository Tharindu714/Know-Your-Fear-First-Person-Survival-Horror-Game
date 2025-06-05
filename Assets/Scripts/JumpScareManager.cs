// File: JumpScareManager.cs
using UnityEngine;
using System.Collections;

public class JumpScareManager : MonoBehaviour
{
    public static JumpScareManager Instance;

    [Header("References")]
    public FearAudioEffects fearAudio;         // → drag your FearAudioEffects here
    public CameraFall cameraFall;              // → drag your CameraFall script here
    public VignetteController vignetteCtrl;    // → drag your VignetteController here
    public FlashlightToggle flashlightToggle;  // → drag your FlashlightToggle here
    public MistController mistController;      // → drag your MistController here
    public PlayerHealth playerHealth;          // → drag your PlayerHealth here (holds currentFear & health)

    [Header("Audio")]
    public AudioSource heavyBreathingSource;   // → drag a looping “heavy breathing” AudioSource (Loop = true, PlayOnAwake = false)

    [Header("Thresholds (Jumpscare Count)")]
    public int thresholdOne = 3;    // at 3 jumpscares → heavy breathing
    public int thresholdTwo = 5;    // at 5 jumpscares → heartbeat + muffle
    public int thresholdFinal = 6;  // at 6 jumpscares → camera fall, red mist, vignette, flashlight off

    [Header("Additional Settings")]
    [Tooltip("How much 'fear' to add to the PlayerHealth.fear meter on each jumpscare")]
    public float fearPerJumpscare = 10f;

    private int _jumpScareCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Call this method whenever ANY jumpscare animation/sound actually plays.
    /// </summary>
    public void RegisterJumpScare()
    {
        _jumpScareCount++;
        Debug.Log($"JumpScareManager: jumpScareCount = {_jumpScareCount}");

        // 1) Add 'fear' to the PlayerHealth meter
        if (playerHealth != null)
        {
            playerHealth.IncreaseFear(fearPerJumpscare);
        }
        
        // 2) Heavy breathing at thresholdOne
        if (_jumpScareCount == thresholdOne)
        {
            if (heavyBreathingSource != null && !heavyBreathingSource.isPlaying)
                heavyBreathingSource.Play();
        }

        // 3) At thresholdTwo, trigger muffle + heartbeat
        if (_jumpScareCount == thresholdTwo)
        {
            if (fearAudio != null)
                fearAudio.TriggerMuffleAndHeartbeat();
        }

        // 4) At thresholdFinal, do camera fall + red mist + vignette + disable flashlight
        if (_jumpScareCount == thresholdFinal)
        {
            // a) Camera Fall
            if (cameraFall != null)
                cameraFall.TriggerFall();

            // b) Red Mist
            if (mistController != null)
                mistController.SetMistColor(Color.red);

            // c) Vignette
            if (vignetteCtrl != null)
                vignetteCtrl.ActivateVignette();

            // d) Disable flashlight permanently
            if (flashlightToggle != null)
                flashlightToggle.ForceDisable();
        }
    }

    private void Update()
    {
        // 5) After thresholdFinal is reached: if player's fear < 50, re-enable flashlight forever
        if (_jumpScareCount >= thresholdFinal && playerHealth != null && flashlightToggle != null)
        {
            if (playerHealth.fear < 50f)
            {
                flashlightToggle.ForceEnable();
            }
        }
    }
}
