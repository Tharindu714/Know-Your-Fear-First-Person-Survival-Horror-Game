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
    public float fearPerJumpscare = 10f;

    private int _jumpScareCount = 0;

    // We only disable candles once in final stage:
    private bool _finalStageTriggered = false;

    // Track whether breathing/heartbeat are currently playing:
    private bool _breathingPlaying = false;
    private bool _heartbeatPlaying = false;

    // Track which jumpscare count actually caused the last trigger, so we don't retrigger on the same count:
    private int _breathTriggeredCount = 0;
    private int _heartTriggeredCount = 0;

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

        // 1) Add fear and damage health
        if (playerHealth != null)
        {
            playerHealth.IncreaseFear(fearPerJumpscare);
            playerHealth.DecreaseHealth(fearPerJumpscare);
        }

        // 2) Check for final stage only if we have reached thresholdFinal count
        if (!_finalStageTriggered && _jumpScareCount >= thresholdFinal)
        {
            float requiredFearForFinal = thresholdTwo * fearPerJumpscare; // e.g. 5 * 10 = 50
            if (playerHealth != null && playerHealth.fear >= requiredFearForFinal)
            {
                TriggerFinalStage();
            }
        }

        // 3) On 8th jumpscare, game over
        if (_jumpScareCount >= killsThreshold)
        {
            GameOverUIManager gom = FindObjectOfType<GameOverUIManager>();
            if (gom != null)
                gom.ShowGameOver();

            // 2) Then immediately disable the Recording UI:
            RecorderUIController recorder = FindObjectOfType<RecorderUIController>();
            if (recorder != null && recorder.recordingUIParent != null)
                recorder.recordingUIParent.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerHealth == null) return;

        float currentFear = playerHealth.fear;
        float fearThresholdOne = thresholdOne * fearPerJumpscare;   // e.g. 3 * 10 = 30
        float fearThresholdTwo = thresholdTwo * fearPerJumpscare;   // e.g. 5 * 10 = 50

        // —— Handle heavy breathing (thresholdOne) ——
        // Only start breathing if we've reached at least thresholdOne jumpscares
        // and we haven't already triggered breathing on this count, and fear ≥ required.
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
        // If fear ever dips below thresholdOne, stop breathing:
        if (_breathingPlaying && currentFear < fearThresholdOne)
        {
            if (heavyBreathingSource != null)
                heavyBreathingSource.Stop();
            _breathingPlaying = false;
        }

        // —— Handle heartbeat & muffle (thresholdTwo) ——
        // Only start heartbeat if we've reached at least thresholdTwo jumpscares
        // and we haven't already triggered heartbeat on this count, and fear ≥ required.
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
        // If fear ever dips below thresholdTwo, stop the heartbeat:
        if (_heartbeatPlaying && currentFear < fearThresholdTwo)
        {
            if (fearAudio != null && fearAudio.heartbeatSource != null)
                fearAudio.heartbeatSource.Stop();
            _heartbeatPlaying = false;
        }

        // —— Re-enable flashlight after final stage if fear < 50 ——
        if (_finalStageTriggered && flashlightToggle != null && currentFear < 50f)
        {
            flashlightToggle.ForceEnable();
        }
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