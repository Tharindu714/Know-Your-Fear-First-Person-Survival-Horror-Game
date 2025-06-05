using UnityEngine;
using UnityEngine.UI;
using TMPro;  // (only if you’re using TextMeshPro for the stopwatch)

public class RecorderUIController : MonoBehaviour
{
    [Header("References (all under your RecordingUI parent)")]
    [Tooltip("Drag the GameObject that holds EVERYTHING for the recording overlay (REC icon, stopwatch, slider, etc.).")]
    public GameObject recordingUIParent;

    [Tooltip("Drag the UI Text (or TextMeshProUGUI) that will show the MM:SS countdown.")]
    public TextMeshProUGUI stopwatchText;    // or use `public Text stopwatchText;` if you’re using legacy Text

    [Tooltip("Drag the UI Slider that represents the recording progress bar.")]
    public Slider recordingSlider;

    [Header("Settings")]
    [Tooltip("Total recording duration in seconds (10 minutes = 600 seconds).")]
    public float totalDuration = 600f;

    [Tooltip("Which key toggles the Recording UI on/off.")]
    public KeyCode toggleKey = KeyCode.R;

    // Internal state
    private float _remainingTime;
    private bool _isRecording = false;

    private void Start()
    {
        // Hide the UI at the very beginning
        if (recordingUIParent != null)
            recordingUIParent.SetActive(false);

        // Initialize timer
        _remainingTime = totalDuration;

        // Initialize slider + stopwatch display
        UpdateStopwatchText(_remainingTime);
        if (recordingSlider != null)
            recordingSlider.value = 0f;
    }

    private void Update()
    {
        // 1) Listen for R key to toggle on/off
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleRecordingUI();
        }

        // 2) If recording is active, count down
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

            // If time has run out, stop recording and optionally hide UI or trigger something
            if (_remainingTime <= 0f)
            {
                EndRecording();
            }
        }
    }

    /// <summary>
    /// Show or hide the recording UI and start/pause the countdown accordingly.
    /// </summary>
    private void ToggleRecordingUI()
    {
        if (recordingUIParent == null)
            return;

        bool isActive = recordingUIParent.activeSelf;
        recordingUIParent.SetActive(!isActive);

        if (!isActive)
        {
            // We just turned it ON
            if (_remainingTime > 0f)
                _isRecording = true;  // start counting down
            else
                _isRecording = false; // if time’s already zero, don’t start
        }
        else
        {
            // We just turned it OFF (pause)
            _isRecording = false;
        }
    }

    /// <summary>
    /// When timer hits zero, this runs once.
    /// You can hide the UI or do other logic here.
    /// </summary>
    private void EndRecording()
    {
        _isRecording = false;

        // Optionally hide the UI automatically when time’s up:
        if (recordingUIParent != null)
            recordingUIParent.SetActive(false);

        // If you want to do something else (e.g. play a “time’s up” sound), do it here.
        Debug.Log("Recording finished.");
    }

    /// <summary>
    /// Converts remainingTime (in seconds) into “MM:SS” and writes to the UI text.
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
}
