using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementManager : MonoBehaviour
{
    // Singleton
    public static AchievementManager I;

    [Header("Reference your five UI panels (with CanvasGroup) here—start them disabled):")]
    public GameObject LetterCollectorUI;
    public GameObject ToyCollectorUI;
    public GameObject MasterJumpscareUI;
    public GameObject SpinAsHellUI;
    public GameObject WinstonTrueColorUI;

    [Header("Timings")]
    public float displayTime = 7f;     // how long to stay fully visible
    public float fadeTime    = 1f;     // how long to fade out

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    // Call these from elsewhere, exactly as before:
    public void OnLetterCollected()       => StartCoroutine(ShowAndHide(LetterCollectorUI));
    public void OnToyCollected()          => StartCoroutine(ShowAndHide(ToyCollectorUI));
    public void OnJumpScare()             => StartCoroutine(ShowAndHide(MasterJumpscareUI));
    public void OnSpin()                  => StartCoroutine(ShowAndHide(SpinAsHellUI));
    public void OnFirstCandleGhost()      => StartCoroutine(ShowAndHide(WinstonTrueColorUI));

    private IEnumerator ShowAndHide(GameObject uiObj)
    {
        if (uiObj == null) yield break;

        // 1) Activate & ensure full alpha
        uiObj.SetActive(true);
        CanvasGroup cg = uiObj.GetComponent<CanvasGroup>();
        if (cg == null) cg = uiObj.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // 2) Wait at full opacity
        yield return new WaitForSeconds(displayTime);

        // 3) Fade out
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }

        // 4) Deactivate and reset alpha
        uiObj.SetActive(false);
        cg.alpha = 1f;
    }
}
