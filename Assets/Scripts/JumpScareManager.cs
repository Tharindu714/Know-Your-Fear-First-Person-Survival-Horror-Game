using UnityEngine;

public class JumpScareManager : MonoBehaviour
{
    public static JumpScareManager Instance;

    [Header("References")]
    public FearAudioEffects fearAudio;          // → drag your FearAudioEffects here
    public CameraFall cameraFall;               // → drag your CameraFall script here
    public VignetteController vignetteCtrl;     // → drag your VignetteController here
    public FlashlightToggle flashlightToggle;   // → drag your FlashlightToggle here
    public MistController mistController;       // → drag your MistController here
    public PlayerHealth playerHealth;           // → drag your PlayerHealth here

    [Header("Audio")]
    public AudioSource heavyBreathingSource;    // → looping “heavy breathing” (Loop = true, PlayOnAwake = false)

    [Header("Thresholds (Jumpscare Count)")]
    public int thresholdOne = 3;                // 3 jumpscares → breathing stage
    public int thresholdTwo = 5;                // 5 jumpscares → heartbeat stage
    public int thresholdFinal = 6;              // 6 jumpscares → final stage (freeze & disable)
    public int killsThreshold = 8;              // 8 jumpscares → game over

    [Header("Additional Settings")]
    [Tooltip("How much 'fear' to add on each jumpscare")]
    public float fearPerJumpscare = 12f;

    // Internal state:
    private int _jumpScareCount = 0;
    private bool _finalStageTriggered = false;
    private bool _breathingPlaying = false;
    private bool _heartbeatPlaying = false;
    private int _breathTriggeredCount = 0;
    private int _heartTriggeredCount = 0;

    // Fear‐based vignette thresholds:
    private const float VIGNETTE_ON_FEAR = 80f;   // when fear ≥ 80 → show vignette
    private const float VIGNETTE_OFF_FEAR = 60f;   // when fear ≤ 60 → hide vignette
    private const float MAX_FEAR_DEATH = 100f;  // when fear reaches 100 → immediate death

    // Whether we've already forced Game Over via fear
    private bool _fearKillingStageTriggered = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Call this whenever a jumpscare animation/sound actually plays.
    /// </summary>
    public void RegisterJumpScare()
    {
        _jumpScareCount++;
        Debug.Log($"JumpScareManager: jumpScareCount = {_jumpScareCount}");

        // Tell the slider manager about this jumpscare:
        JumpscareSliderManager.I?.OnJumpScare();
        
        // 1) Add fear and damage health
        if (playerHealth != null)
        {
            playerHealth.IncreaseFear(fearPerJumpscare);
            playerHealth.DecreaseHealth(fearPerJumpscare);
        }

        // 2) Check for fear‐based immediate death:
        if (!_fearKillingStageTriggered && playerHealth != null && playerHealth.fear >= MAX_FEAR_DEATH)
        {
            // Player has lost all sanity → Game Over:
            _fearKillingStageTriggered = true;
            TriggerFearDeath();
            return; // no further jumpscare logic needed
        }

        // 3) Check for final stage only if we have reached thresholdFinal count
        if (!_finalStageTriggered && _jumpScareCount >= thresholdFinal)
        {
            float requiredFearForFinal = thresholdTwo * fearPerJumpscare; // e.g. 5 * 12 = 60
            if (playerHealth != null && playerHealth.fear >= requiredFearForFinal)
            {
                TriggerFinalStage();
                AchievementManager.I.OnJumpScare();
            }
        }

        // 4) On 8th jumpscare, game over
        if (_jumpScareCount >= killsThreshold)
        {
            TriggerGameOver();
        }
    }

    private void Update()
    {
        if (playerHealth == null) return;

        float currentFear = playerHealth.fear;
        float fearThresholdOne = thresholdOne * fearPerJumpscare;   // e.g. 3 * 12 = 36
        float fearThresholdTwo = thresholdTwo * fearPerJumpscare;   // e.g. 5 * 12 = 60

        // ——— Handle heavy breathing (thresholdOne) ———
        if (_jumpScareCount >= thresholdOne &&
            _jumpScareCount > _breathTriggeredCount &&
            currentFear >= fearThresholdOne)
        {
            if (heavyBreathingSource != null && !_breathingPlaying)
            {
                heavyBreathingSource.Play();
                _breathingPlaying = true;
                _breathTriggeredCount = _jumpScareCount;
            }
        }
        if (_breathingPlaying && currentFear < fearThresholdOne)
        {
            if (heavyBreathingSource != null)
                heavyBreathingSource.Stop();
            _breathingPlaying = false;
        }

        // ——— Handle heartbeat & muffle (thresholdTwo) ———
        if (_jumpScareCount >= thresholdTwo &&
            _jumpScareCount > _heartTriggeredCount &&
            currentFear >= fearThresholdTwo)
        {
            if (fearAudio != null && !_heartbeatPlaying)
            {
                fearAudio.TriggerMuffleAndHeartbeat();
                _heartbeatPlaying = true;
                _heartTriggeredCount = _jumpScareCount;
            }
        }
        if (_heartbeatPlaying && currentFear < fearThresholdTwo)
        {
            if (fearAudio != null && fearAudio.heartbeatSource != null)
                fearAudio.heartbeatSource.Stop();
            _heartbeatPlaying = false;
        }

        // ——— Re‐enable flashlight after final stage if fear < 50 ———
        if (_finalStageTriggered && flashlightToggle != null && currentFear < 50f)
        {
            flashlightToggle.ForceEnable();
        }

        // ——— Handle fear‐based vignette (even without a jumpscare) ———
        if (!_fearKillingStageTriggered)
        {
            if (currentFear >= VIGNETTE_ON_FEAR)
            {
                // show red vignette
                if (vignetteCtrl != null)
                    vignetteCtrl.ActivateVignette();
            }
            else if (currentFear <= VIGNETTE_OFF_FEAR)
            {
                // hide red vignette
                if (vignetteCtrl != null)
                    vignetteCtrl.DeactivateVignette();
            }
        }

        // ——— If fear hits 100 outside a jumpscare, kill the player now ———
        if (!_fearKillingStageTriggered && currentFear >= MAX_FEAR_DEATH)
        {
            _fearKillingStageTriggered = true;
            TriggerFearDeath();
        }
    }

    /// <summary>
    /// Called when fear reaches 100%—forces Game Over.
    /// </summary>
    private void TriggerFearDeath()
    {
        Debug.Log("Player has lost all sanity—Game Over.");
        // Show Game Over UI:
        GameOverUIManager gom = FindObjectOfType<GameOverUIManager>();
        if (gom != null) gom.ShowGameOver();

        // Also immediately disable recording UI if it’s on:
        RecorderUIController recorder = FindObjectOfType<RecorderUIController>();
        if (recorder != null && recorder.recordingUIParent != null)
            recorder.recordingUIParent.SetActive(false);
    }

    /// <summary>
    /// Called when we hit killsThreshold (8) or explicitly from fear death.
    /// </summary>
    private void TriggerGameOver()
    {
        Debug.Log("8 jumpscares reached—Game Over.");
        GameOverUIManager gom = FindObjectOfType<GameOverUIManager>();
        if (gom != null) gom.ShowGameOver();

        RecorderUIController recorder = FindObjectOfType<RecorderUIController>();
        if (recorder != null && recorder.recordingUIParent != null)
            recorder.recordingUIParent.SetActive(false);
    }

    private void TriggerFinalStage()
    {
        _finalStageTriggered = true;

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

        // e) Disable all CandleControllers so player cannot heal or light them
        DisableAllCandles();
    }

    /// <summary>
    /// Finds every CandleController in the scene and disables it.
    /// </summary>
    private void DisableAllCandles()
    {
        CandleController[] allCandles = FindObjectsOfType<CandleController>();
        foreach (var candle in allCandles)
        {
            if (candle.isLit)
                candle.ToggleCandle();
            candle.enabled = false;
        }
    }
}
