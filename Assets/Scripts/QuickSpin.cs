using UnityEngine;
using System.Collections;

public class QuickSpin : MonoBehaviour
{
    public KeyCode spinKey = KeyCode.C;
    public float spinDuration = 0.2f;

    private bool _isSpinning = false;
    private int  _consecutiveSpins = 0;  // track back-to-back spins

    void Update()
    {
        if (!_isSpinning 
         && FollowingGhostManager.PanicActive 
         && Input.GetKeyDown(spinKey))
        {
            StartCoroutine(DoSpin());
        }
    }

    private IEnumerator DoSpin()
    {
        _isSpinning = true;

        Quaternion startRot  = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0f, 180f, 0f);

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / spinDuration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;
        _isSpinning = false;

        // —— NEW: count consecutive spins —— 
        _consecutiveSpins++;

        if (_consecutiveSpins >= 5)
        {
            // after the 5th spin in a row, fire the achievement
            AchievementManager.I.OnSpin();

            // reset so you need another 5 in a row next time
            _consecutiveSpins = 0;
        }
    }
}
