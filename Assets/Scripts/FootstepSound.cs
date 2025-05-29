using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (footstepSource == null) return;

        // only horizontal movement, ignore vertical velocity
        Vector3 hVel = new Vector3(cc.velocity.x, 0f, cc.velocity.z);
        bool walking = hVel.magnitude > minMoveSpeed && cc.isGrounded;

        if (walking && !footstepSource.isPlaying)
            footstepSource.Play();
        else if (!walking && footstepSource.isPlaying)
            footstepSource.Stop();
    }
}

