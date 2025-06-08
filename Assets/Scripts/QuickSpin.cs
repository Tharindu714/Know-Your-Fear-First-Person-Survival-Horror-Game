using UnityEngine;
using System.Collections;

public class QuickSpin : MonoBehaviour
{
    [Tooltip("Key to perform the quick 180° turn.")]
    public KeyCode spinKey = KeyCode.C;

    [Tooltip("How long the spin animation should take.")]
    public float spinDuration = 0.2f;

    // Are we mid‐spin right now?
    private bool _isSpinning = false;

    void Update()
    {
        // If panic is active, not already spinning, and player presses C → start spin
        if (! _isSpinning 
         && FollowingGhostManager.PanicActive 
         && Input.GetKeyDown(spinKey))
        {
            StartCoroutine(DoSpin());
        }
    }

    private IEnumerator DoSpin()
    {
        _isSpinning = true;

        // Capture start & target rotations
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

        // Snap exactly to target
        transform.rotation = targetRot;
        _isSpinning = false;
    }
}
