using UnityEngine;

public class PhotoInteraction : MonoBehaviour
{
    [Tooltip("Assign the Light (or GameObject) that illuminates this photo.")]
    public Light photoLight;        // Or use GameObject if you disabled a point light GO

    private bool _isInteracted = false;

    /// <summary>
    /// Call this to "turn off" the light when the player interacts.
    /// </summary>
    public void Interact()
    {
        if (_isInteracted) return;
        _isInteracted = true;

        if (photoLight != null)
            photoLight.enabled = false;

        // Optionally play a click / off‐switch sound here
        // AudioSource.PlayClipAtPoint(offClip, transform.position);
    }
}
