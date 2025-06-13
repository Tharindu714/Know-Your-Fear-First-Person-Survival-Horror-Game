// Assets/Editor/ConvertMaterialsToStandard.cs
using UnityEngine;
using UnityEditor;

public class ConvertMaterialsToStandard
{
    [MenuItem("Tools/Convert All Materials → Standard")]
    static void ConvertAll()
    {
        var mats = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        foreach (var guid in mats)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat  = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            // Only convert URP shaders (or however you want to filter)
            if (mat.shader.name.StartsWith("Universal Render Pipeline"))
            {
                mat.shader = Shader.Find("Standard");
                EditorUtility.SetDirty(mat);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Re-assigned {count} materials to Standard shader.");
    }
}
