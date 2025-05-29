using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager I;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;

    [Header("Scene Names (optional)")]
    public string gameSceneName;    // if you want to load a separate scene for gameplay
    public string menuSceneName;    // if your main menu is in its own scene

    void Awake()
    {
        // Simple singleton
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    // Called from FearManager when player dies
    public void ShowGameOver()
    {
        // Hide menu if it was open
        mainMenuPanel.SetActive(false);
        // Show Game Over
        gameOverPanel.SetActive(true);
        // Pause time
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Hook this to your Game Over “Main Menu” button
    public void OnGameOver_MainMenu()
    {
        // Option A: Toggle panels in the same scene
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        // Still paused until you hit Play
    }

    // Hook this to your Main Menu “Play” button
    public void OnMainMenu_Play()
    {
        // Option A: Same scene – just unpause and hide menu
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // (Optional) If you have an actual menu scene, call this from a UI button:
    public void OnQuitToDesktop()
    {
        Application.Quit();
    }
}
