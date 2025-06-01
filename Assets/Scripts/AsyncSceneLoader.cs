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

    [Header("Story Prefab")]
    [Tooltip("Drag the StoryText prefab here (will be spawned after Continue)")]
    public GameObject storyTextPrefab;


    [Header("Scene Names")]
    [Tooltip("Name of the actual gameplay scene")]
    public string gameplaySceneName = "Playground";

    // Keep a reference to the instantiated story text
    private GameObject _storyInstance;

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
        progressPanel.SetActive(true);

        // Spawn the story prefab in the middle of the ProgressPanel
        // if (storyTextPrefab != null && _storyInstance == null)
        // {
        //     // Instantiate as child of progressPanel
        //     _storyInstance = Instantiate(storyTextPrefab, progressPanel.transform);
        //     // Optionally reset its localPosition to (0,0) so it’s centered:
        //     _storyInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        // }


        // Initialize slider and text
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
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

    float displayedProgress = 0f;

    while (!op.isDone)
    {
        // Unity returns op.progress in [0, 0.9]
        float targetProgress = Mathf.Clamp01(op.progress / 0.9f);

        // Smoothly move the displayed progress toward the real one
        displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 0.25f);

        if (progressSlider != null)
            progressSlider.value = displayedProgress;

        if (progressText != null)
            progressText.text = $"{Mathf.RoundToInt(displayedProgress * 100f)}%";

        // When real load is done and displayed bar reaches 100%
        if (op.progress >= 0.9f && displayedProgress >= 0.99f)
        {
            if (progressSlider != null)
                progressSlider.value = 1f;

            if (progressText != null)
                progressText.text = "100%";

            yield return new WaitForSeconds(0.5f);
            op.allowSceneActivation = true;
        }

        yield return null;
    }
}

}