// Assets/Editor/FixPinkMaterials.cs
using UnityEngine;
using UnityEditor;

public class FixPinkMaterials : MonoBehaviour
{
    [MenuItem("Tools/Fix Pink Materials in Project")]
    public static void FixMaterials()
    {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        int fixedCount = 0;

        foreach (string guid in materialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null) continue;

            // Detect broken (pink) or URP materials
            bool isURP = mat.shader != null && mat.shader.name.StartsWith("Universal Render Pipeline");
            bool isPink = mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader";

            if (isURP || isPink)
            {
                Shader standard = Shader.Find("Standard");
                if (standard != null)
                {
                    mat.shader = standard;
                    EditorUtility.SetDirty(mat);
                    fixedCount++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ Fixed {fixedCount} material(s) to Standard shader.");
    }
}
