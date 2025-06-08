using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    [Header("Panels (disable these in Editor)")]
    [Tooltip("Drag the GameOverPanel here.")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Drag the WinPanel here.")]
    [SerializeField] private GameObject winPanel;

    [Header("Scene Names")]
    [Tooltip("Name of your Main Menu scene.")]
    [SerializeField] private string menuSceneName = "Menu";

    private void Awake()
    {
        // Ensure both panels start hidden
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel      != null) winPanel.SetActive(false);
    }

    /// <summary>
    /// Call this to show the Game Over screen (loss).
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        PauseAndUnlockCursor();
    }

    /// <summary>
    /// Call this to show the Win screen (victory).
    /// </summary>
    public void ShowWin()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        PauseAndUnlockCursor();
    }

    /// <summary>
    /// Shared logic for pausing and unlocking the cursor.
    /// </summary>
    private void PauseAndUnlockCursor()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Hook this to any “Back to Menu” button inside either panel.
    /// </summary>
    public void OnBackToMenuPressed()
    {
        // Resume time so the menu scene runs normally
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}