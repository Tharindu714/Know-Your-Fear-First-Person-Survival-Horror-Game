using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        Material mat = rend.material;

        // Use Transparent rendering mode
        mat.SetFloat("_Mode", 3); // Transparent
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        // Set transparent color
        Color color = mat.color;
        color.a = 0.3f; // 30% visible
        mat.color = color;
    }
}
