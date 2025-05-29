using UnityEngine;

public class GiftBox : MonoBehaviour
{
    [Header("Box Parts")]
    [SerializeField] private GameObject closedBox;  // child with “Close” model
    [SerializeField] private GameObject openBox;    // child with “Open” model

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSfx;

    // Tracks whether we've already opened it
    public bool _isOpen = false;

    /// <summary>
    /// Call this when the player presses O.
    /// </summary>
    public void OpenBox()
    {
        // If already opened, do nothing
        if (_isOpen) return;

        // Only open if this GameObject is still tagged "Close"
        if (!gameObject.CompareTag("Close"))
            return;

        // Play SFX
        if (audioSource != null && openSfx != null)
            AudioManager.I.sfxSource.PlayOneShot(openSfx);

        // Swap visuals
        if (closedBox != null) closedBox.SetActive(false);
        if (openBox != null) openBox.SetActive(true);

        // Mark as opened so we don’t run twice
        _isOpen = true;

        // Disable collider so you can’t interact again
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Change the tag so your Interaction logic will skip it
        gameObject.tag = "Open";
    }
}


