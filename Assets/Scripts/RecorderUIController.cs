using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Only if you’re using TextMeshPro for the stopwatch

public class RecorderUIController : MonoBehaviour
{
    [Header("References (all under your RecordingUI parent)")]
    [Tooltip("Drag the GameObject that holds EVERYTHING for the recording overlay (REC icon, stopwatch, slider, etc.).")]
    public GameObject recordingUIParent;

    [Tooltip("Drag the UI TextMeshProUGUI that will show the MM:SS countdown.")]
    public TextMeshProUGUI stopwatchText;    // If you’re using legacy Text instead, change this to `public Text stopwatchText;`

    [Tooltip("Drag the UI Slider that represents the recording progress bar.")]
    public Slider recordingSlider;

    [Header("Night Vision")]
    [Tooltip("Drag the NightVision GameObject (parent of your Spot Light) here.")]
    public GameObject nightVisionObject;

    [Tooltip("Drag an AudioSource that contains your 2-second Night Vision on sound (PlayOnAwake = false).")]
    public AudioSource nightVisionAudioSource;

    [Header("Settings")]
    [Tooltip("Total recording duration in seconds (10 minutes = 600 seconds).")]
    public float totalDuration = 600f;

    [Tooltip("Key to toggle the Recording UI on/off.")]
    public KeyCode toggleKey = KeyCode.R;

    [Tooltip("Key to toggle Night Vision on/off when the Recording UI is active.")]
    public KeyCode nightVisionKey = KeyCode.N;

    // Internal state
    private float _remainingTime;
    private bool _isRecording = false;
    private bool _isNightVisionOn = false;

    private void Start()
    {
        // Hide the Recording UI at the very beginning
        if (recordingUIParent != null)
            recordingUIParent.SetActive(false);

        // Ensure Night Vision is off at start
        if (nightVisionObject != null)
            nightVisionObject.SetActive(false);

        // Initialize timer
        _remainingTime = totalDuration;

        // Initialize slider + stopwatch display
        UpdateStopwatchText(_remainingTime);

        if (recordingSlider != null)
            recordingSlider.value = 0f;
    }

    private void Update()
    {
        // 1) Listen for R key to toggle Recording UI on/off
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleRecordingUI();
        }

        // 2) If Recording UI is active, listen for N key to toggle Night Vision
        if (recordingUIParent != null && recordingUIParent.activeSelf)
        {
            if (Input.GetKeyDown(nightVisionKey))
            {
                ToggleNightVision();
            }
        }

        // 3) If recording is active, count down
        if (_isRecording)
        {
            // Subtract the time that has passed since last frame
            _remainingTime -= Time.deltaTime;

            // Clamp to zero
            if (_remainingTime < 0f)
                _remainingTime = 0f;

            // Update the stopwatch text
            UpdateStopwatchText(_remainingTime);

            // Update the slider: value goes from 0 (start) to 1 (end)
            if (recordingSlider != null)
            {
                float t = 1f - (_remainingTime / totalDuration);
                recordingSlider.value = t;
            }

            // If time has run out, stop recording and hide UI
            if (_remainingTime <= 0f)
            {
                EndRecording();
            }
        }
    }

    /// <summary>
    /// Show or hide the Recording UI and start/pause the countdown accordingly.
    /// </summary>
    private void ToggleRecordingUI()
    {
        if (recordingUIParent == null)
            return;

        bool wasActive = recordingUIParent.activeSelf;
        recordingUIParent.SetActive(!wasActive);

        if (!wasActive)
        {
            // We just turned Recording UI ON
            if (_remainingTime > 0f)
                _isRecording = true;  // start counting down
            else
                _isRecording = false; // if time’s already zero, don’t start
        }
        else
        {
            // We just turned Recording UI OFF (pause)
            _isRecording = false;

            // Also, if Night Vision was on, turn it off
            if (_isNightVisionOn)
                ToggleNightVision();
        }
    }

    /// <summary>
    /// Toggle Night Vision on/off and play the 2-second audio when turning on.
    /// </summary>
    private void ToggleNightVision()
    {
        if (nightVisionObject == null)
            return;

        _isNightVisionOn = !_isNightVisionOn;
        nightVisionObject.SetActive(_isNightVisionOn);

        if (_isNightVisionOn)
        {
            // Play the Night Vision activation sound
            if (nightVisionAudioSource != null)
                nightVisionAudioSource.Play();
        }
        else
        {
            // Stop the audio if it's still playing
            if (nightVisionAudioSource != null && nightVisionAudioSource.isPlaying)
                nightVisionAudioSource.Stop();
        }
    }

    /// <summary>
    /// When timer hits zero, this runs once. Hides UI and turns off Night Vision.
    /// </summary>
    private void EndRecording()
    {
        _isRecording = false;

        // Hide the Recording UI
        if (recordingUIParent != null)
            recordingUIParent.SetActive(false);

        // Turn off Night Vision if it’s on
        if (_isNightVisionOn)
            ToggleNightVision();

        Debug.Log("Recording finished.");
    }

    /// <summary>
    /// Converts remainingTime (in seconds) into “MM:SS” and writes it to the UI text.
    /// </summary>
    private void UpdateStopwatchText(float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        if (stopwatchText != null)
        {
            stopwatchText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    /// <summary>
    /// Call this from other scripts (e.g. on player death or win) to immediately hide everything.
    /// </summary>
    public void DisableRecordingUI()
    {
        _isRecording = false;

        if (recordingUIParent != null)
            recordingUIParent.SetActive(false);

        if (_isNightVisionOn)
            ToggleNightVision();
    }
}
