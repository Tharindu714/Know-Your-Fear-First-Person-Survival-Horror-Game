using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    [Tooltip("Drag the GameOverPanel here (disabled at start)")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Name of your Menu scene")]
    [SerializeField] private string menuSceneName = "Menu";

    /// <summary>
    /// Call this to show the Game Over screen.
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    /// <summary>
    /// Hook this to the “Back to Menu” button inside GameOverPanel.
    /// </summary>
    public void OnBackToMenuPressed()
    {
        // Resume time (important so Menu isn't paused)
        Time.timeScale = 1f;

        // Load the Menu scene
        SceneManager.LoadScene(menuSceneName);
    }
}
