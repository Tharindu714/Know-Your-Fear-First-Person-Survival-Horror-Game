using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepSound : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Looping footstep clip")]
    public AudioSource footstepSource;

    [Header("Movement")]
    [Tooltip("Minimum horizontal speed to play footsteps")]
    public float minMoveSpeed = 0.1f;

    private CharacterController cc;
    private CameraFall camFall;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        camFall = FindObjectOfType<CameraFall>();

        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (footstepSource == null || camFall == null) return;

        // If the player is currently “down,” never play footsteps
        if (camFall.IsDown)
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
            return;
        }

        // Only horizontal movement, ignore vertical velocity
        Vector3 hVel = new Vector3(cc.velocity.x, 0f, cc.velocity.z);
        bool walking = hVel.magnitude > minMoveSpeed && cc.isGrounded;

        if (walking && !footstepSource.isPlaying)
            footstepSource.Play();
        else if (!walking && footstepSource.isPlaying)
            footstepSource.Stop();
    }
}
