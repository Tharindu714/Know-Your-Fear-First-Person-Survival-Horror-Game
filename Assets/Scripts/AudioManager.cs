using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;      // static instance

    [Tooltip("Drag your 2D SFX AudioSource here")]
    public AudioSource sfxSource;

    void Awake()
    {
        // simple singleton pattern
        if (I == null) I = this;
        else Destroy(gameObject);

        // persist across scenes if needed
        // DontDestroyOnLoad(gameObject);
    }
}

