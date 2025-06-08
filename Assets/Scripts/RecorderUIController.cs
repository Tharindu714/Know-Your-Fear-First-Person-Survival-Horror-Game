using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;  // for restart button

public class RecorderUIController : MonoBehaviour
{
    [Header("References (all under your RecordingUI parent)")]
    public GameObject recordingUIParent;
    public TextMeshProUGUI stopwatchText;
    public Slider recordingSlider;

    [Header("Night Vision")]
    public GameObject nightVisionObject;
    public AudioSource nightVisionAudioSource;

    [Header("Game Over")]
    [Tooltip("Drag your GameOverUIManager here.")]
    public GameOverUIManager gameOverUI;
    [Tooltip("AudioSource used for playing the Game Over BGM.")]
    public AudioSource bgmSource;
    [Tooltip("Looped BGM clip to play at Game Over.")]
    public AudioClip gameOverBGM;

    [Header("Settings")]
    public float totalDuration = 600f;
    public KeyCode toggleKey = KeyCode.R;
    public KeyCode nightVisionKey = KeyCode.N;

    private float _remainingTime;
    private bool  _isRecording = false;
    private bool  _isNightVisionOn = false;
    public static RecorderUIController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        recordingUIParent.SetActive(false);
        nightVisionObject.SetActive(false);

        _remainingTime = totalDuration;
        UpdateStopwatchText(_remainingTime);
        recordingSlider.value = 0f;
    }

    private void Update()
    {
        if (!Letter.IsReading && !Instruction.IsReading && Input.GetKeyDown(toggleKey))
            ToggleRecordingUI();

        if (recordingUIParent.activeSelf && Input.GetKeyDown(nightVisionKey))
            ToggleNightVision();

        if (_isRecording)
        {
            _remainingTime -= Time.deltaTime;
            if (_remainingTime < 0f) _remainingTime = 0f;

            UpdateStopwatchText(_remainingTime);
            recordingSlider.value = 1f - (_remainingTime / totalDuration);

            if (_remainingTime <= 0f)
                TriggerRecordingGameOver();
        }
    }

    private void ToggleRecordingUI()
    {
        bool wasActive = recordingUIParent.activeSelf;
        recordingUIParent.SetActive(!wasActive);

        _isRecording = !wasActive && _remainingTime > 0f;
        if (wasActive && _isNightVisionOn)
            ToggleNightVision();
    }

    private void ToggleNightVision()
    {
        _isNightVisionOn = !_isNightVisionOn;
        nightVisionObject.SetActive(_isNightVisionOn);

        if (_isNightVisionOn) nightVisionAudioSource?.Play();
        else if (nightVisionAudioSource.isPlaying) nightVisionAudioSource.Stop();
    }

    /// <summary>
    /// Called when the countdown hits zero.
    /// </summary>
    private void TriggerRecordingGameOver()
    {
        _isRecording = false;
        recordingUIParent.SetActive(false);
        if (_isNightVisionOn) ToggleNightVision();

            gameOverUI.ShowWin();

        // 2) Play the BGM
        if (bgmSource != null && gameOverBGM != null)
        {
            bgmSource.clip = gameOverBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// When timer hits zero via code, call here instead of EndRecording.
    /// </summary>
    private void EndRecording()
    {
        // you can leave this empty or remove it
    }

    private void UpdateStopwatchText(float remainingTime)
    {
        int m = Mathf.FloorToInt(remainingTime / 60f);
        int s = Mathf.FloorToInt(remainingTime % 60f);
        stopwatchText.text = $"{m:00}:{s:00}";
    }

    /// <summary>
    /// Hide everything immediately (e.g. on manual game over).
    /// </summary>
    public void DisableRecordingUI()
    {
        _isRecording = false;
        recordingUIParent.SetActive(false);
        if (_isNightVisionOn) ToggleNightVision();
    }
}
