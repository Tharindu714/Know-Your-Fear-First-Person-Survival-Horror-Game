using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Lives Settings")]
    public int maxLives = 3;
    [HideInInspector] public int currentLives;

    [Header("Mist Settings")]
    public ParticleSystem mistPS;       // assign your mist ParticleSystem
    public Color redMistColor = Color.red;

    [Header("UI")]
    public GameObject gameOverUI;       // assign your Game Over panel (disabled by default)

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentLives = maxLives;
    }

    /// <summary>Call whenever player “dies” (monster attack).</summary>
    public void LoseLife()
    {
        int prevLives = currentLives;
        currentLives--;

        // If this was the second “fall,” disable torch:
        // (maxLives=3, prevLives=2→1 is second fall)
        if (prevLives - currentLives == 1 && currentLives == maxLives - 2)
        {
            // permanently turn off torch
            FlashlightToggle ft = FindObjectOfType<FlashlightToggle>();
            if (ft != null)
            {
                ft.DisableForever();
                VignetteController.I?.FadeInRed(2f);

            }
        }

        if (currentLives <= 0)
        {
            GameOver();
        }
    }


    private void GameOver()
    {
        // Show Game Over UI
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Freeze time if desired
        Time.timeScale = 0f;
    }

    /// <summary>Call this from Game Over “Restart” button or similar.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
