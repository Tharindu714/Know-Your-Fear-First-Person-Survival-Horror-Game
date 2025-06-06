using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JumpscareSliderManager : MonoBehaviour
{
    public static JumpscareSliderManager I;

    [Header("UI")]
    [Tooltip("Drag the UI Slider (0→1) that visually fills as jumpscares accumulate.")]
    public Slider jumpscareSlider;

    [Header("References")]
    [Tooltip("Drag your PlayerHealth component here.")]
    public PlayerHealth playerHealth;
    [Tooltip("Drag your CameraFall component here.")]
    public CameraFall cameraFall;
    [Tooltip("Drag your GameOverUIManager here.")]
    public GameOverUIManager gameOverUI;

    [Header("Settings")]
    [Tooltip("How many jumpscares to fill slider to 100%.")]
    public int jumpscaresToFill = 7;

    [Tooltip("How many seconds to wait after camera fall before showing Game Over.")]
    public float secondsBeforeGameOver = 5f;

    private int _currentCount = 0;
    private bool _triggered = false;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Ensure slider starts at zero
        if (jumpscareSlider != null)
            jumpscareSlider.value = 0f;
        // Self-assign references if not set in Inspector
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();
        if (cameraFall == null)
            cameraFall = FindObjectOfType<CameraFall>();
        if (gameOverUI == null)
            gameOverUI = FindObjectOfType<GameOverUIManager>();
    }

    /// <summary>
    /// Call this method whenever a jumpscare occurs.
    /// </summary>
    public void OnJumpScare()
    {
        if (_triggered) return;

        _currentCount++;
        // Update slider: normalize 0→1 over 'jumpscaresToFill'
        if (jumpscareSlider != null)
            jumpscareSlider.value = Mathf.Clamp01((float)_currentCount / jumpscaresToFill);

        if (_currentCount >= jumpscaresToFill)
        {
            _triggered = true;
            StartCoroutine(HandleFullJumpscare());
        }
    }

    private IEnumerator HandleFullJumpscare()
    {
        // 1) Force fear to 100 and health to 0
        if (playerHealth != null)
        {
            playerHealth.fear = 100f;
            playerHealth.currentHealth = 0f;
            UIManager.I?.SetFear(100f);
            UIManager.I?.SetHealth(0f);
        }

        // 2) Trigger camera fall
        if (cameraFall != null)
            cameraFall.TriggerFall();

        // 3) Wait for a few seconds
        yield return new WaitForSeconds(secondsBeforeGameOver);

        // 4) Show Game Over
        if (gameOverUI != null)
            gameOverUI.ShowGameOver();
    }
}

