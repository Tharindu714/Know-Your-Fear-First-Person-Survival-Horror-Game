using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AsyncSceneLoader : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("Drag your InstructionsPanel here (active at start)")]
    public GameObject instructionsPanel;
    [Tooltip("Drag your ProgressPanel here (disabled at start)")]
    public GameObject progressPanel;

    [Header("Progress UI")]
    [Tooltip("Drag your Slider UI here")]
    public Slider progressSlider;
    [Tooltip("Drag your TMP Text UI here for percentage")]
    public TMP_Text progressText;

    [Header("Scene Names")]
    [Tooltip("Name of the actual gameplay scene")]
    public string gameplaySceneName = "Playground";

    void Start()
    {
        // Make sure only instructions are visible at start
        instructionsPanel.SetActive(true);
        progressPanel.SetActive(false);
    }

    /// <summary>
    /// Hook this to your “Continue” button in the InstructionsPanel.
    /// </summary>
    public void OnContinuePressed()
    {
        // Hide instructions, show progress UI
        instructionsPanel.SetActive(false);
        progressPanel.SetActive(true);

        // Initialize slider and text
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value    = 0f;
        }
        if (progressText != null)
            progressText.text = "0%";

        // Begin loading the gameplay scene
        StartCoroutine(LoadGameplayAsync());
    }

    private IEnumerator LoadGameplayAsync()
    {
        var op = SceneManager.LoadSceneAsync(gameplaySceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            // Unity reports op.progress from 0 to 0.9; at 0.9 it's basically ready
            float normalized = Mathf.Clamp01(op.progress / 0.9f);
            if (progressSlider != null)
                progressSlider.value = normalized;
            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(normalized * 100f)}%";

            // Once it reaches 0.9 (i.e. 100%), finalize
            if (op.progress >= 0.9f)
            {
                progressSlider.value = 1f;
                if (progressText != null)
                    progressText.text = "100%";

                // Brief pause so player sees “100%”
                yield return new WaitForSeconds(0.5f);

                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}