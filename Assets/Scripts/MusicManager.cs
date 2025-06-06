using UnityEngine;

public class MusicManager : MonoBehaviour 
{
    public static MusicManager I;

    [Header("Jumpscare Cue (play 2 seconds before a jumpscare)")]
    [Tooltip("Assign a short audio clip that plays 2 seconds before any jumpscare.")]
    public AudioClip jumpscareCueClip;

    private AudioSource _jumpscareCueSource;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);

            // Set up a dedicated AudioSource for playing the jumpscare cue
            _jumpscareCueSource = gameObject.AddComponent<AudioSource>();
            _jumpscareCueSource.playOnAwake = false;
            _jumpscareCueSource.loop = false;
            _jumpscareCueSource.clip = jumpscareCueClip;
            _jumpscareCueSource.spatialBlend = 0f; 
                // 0 = 2D (so it plays uniformly, not 3D positional)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call this to immediately play the jumpscare cue (if not already playing).
    /// </summary>
    public void PlayJumpCue()
    {
        if (_jumpscareCueSource == null || jumpscareCueClip == null) return;

        // If it’s already playing (rare), restart from the top:
        _jumpscareCueSource.Stop();
        _jumpscareCueSource.Play();
    }
}
