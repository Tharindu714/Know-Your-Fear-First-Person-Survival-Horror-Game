using UnityEngine;

public class MistController : MonoBehaviour
{
    public ParticleSystem mistParticles;   // → drag your mist ParticleSystem here

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color redColor = Color.red;

    private ParticleSystem.MainModule mainModule;

    private void Start()
    {
        if (mistParticles != null)
            mainModule = mistParticles.main;

        // Ensure it starts at normal color
        if (mistParticles != null)
            mainModule.startColor = normalColor;
    }

    /// <summary>Call this to set the mist color (e.g. red on final jumpscare).</summary>
    public void SetMistColor(Color c)
    {
        if (mistParticles == null) return;
        mainModule.startColor = c;
    }
}
