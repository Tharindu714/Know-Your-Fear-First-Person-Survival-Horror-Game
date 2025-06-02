using UnityEngine;
using System.Collections;

public class FearManager : MonoBehaviour
{
    public static FearManager I;

    [Header("Thresholds")]
    public int breathThreshold = 5;
    public int shakeThreshold = 8;
    public int blurThreshold = 10;
    public int redThreshold = 12;
    public int panicThreshold = 12;

    [Header("Fall Settings")]
    [Tooltip("Fall once every N scares")] public int fallThreshold = 3;
    [Tooltip("Number of falls before game over")] public int maxFalls = 3;

    [Header("Audio")]
    public AudioClip breathingClip;
    public AudioSource breathingSourcePrefab;

    public int _jumpscareCount = 0;
    private int _fallCount = 0;

    private CameraShake _cameraShake;
    private FearEffectsManager _effectsManager;
    private TorchController _torchController;
    private CameraFall _camFall;
    private StaggerMovement _stagger;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        _effectsManager = FindObjectOfType<FearEffectsManager>();
        _torchController = FindObjectOfType<TorchController>();
        _camFall = FindObjectOfType<CameraFall>();
        _stagger = FindObjectOfType<StaggerMovement>();
    }

    /// <summary>
    /// Call this whenever a jumpscare occurs.
    /// </summary>
    public void NotifyJumpscare()
    {
        _jumpscareCount++;

        // Heavy breathing at threshold
        if (_jumpscareCount == breathThreshold)
            PlayBreathing();

        // Camera shake at threshold
        if (_jumpscareCount == shakeThreshold)
            _cameraShake?.TriggerShake(2f, 0.1f, 20f);

        // Blur & vignette
        _effectsManager?.OnFearUpdated(_jumpscareCount);

        // Heartbeat & muffle
        FindObjectOfType<FearAudioEffects>()?.TriggerMuffleAndHeartbeat();

        // Handle periodic falls
        if (_jumpscareCount % fallThreshold == 0)
        {
            _fallCount++;

            if (_fallCount >= maxFalls)
            {
                // Game over logic here
                FindObjectOfType<GameOverUIManager>()?.ShowGameOver();
            }
            else
            {
                // Collapse camera and flicker torch
                _camFall?.FallDown();
                _torchController?.FlickerToWhiteThenBack();
                StartCoroutine(DelayedStagger());
            }
        }
    }

    private IEnumerator DelayedStagger()
    {
        // wait for fall animation to finish
        yield return new WaitForSeconds(_camFall.fallDuration + 0.1f);
        _stagger?.DoStagger();
    }

    private void PlayBreathing()
    {
        if (breathingClip == null || breathingSourcePrefab == null) return;
        var cam = Camera.main.transform;
        var src = Instantiate(breathingSourcePrefab, cam.position, Quaternion.identity);
        src.clip = breathingClip;
        src.loop = true;
        src.spatialBlend = 1f;
        src.Play();
    }
    
    public void NotifyGaze(float amount)
{
    // increment fear by amount
    _jumpscareCount += (int)amount; // example: ramp up jumpscareCount
    // Or if you have a separate “fear” float, just AddFear(amount)
}

    public void LoseLife()
    {
        // Handle losing a life, e.g. decrement player lives
        // GameManager.I.DecrementLives();
        // Reset fear effects if needed
        _jumpscareCount = 0;
        // _effectsManager?.ResetEffects();
    }
}
