using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class MaterialGPUInstancing : MonoBehaviour
{
    [MenuItem("Custom/GPUMaterialInstancing")]
    public static void GPUMaterialInstancing()
    {
        foreach (var mat in Resources.FindObjectsOfTypeAll<Material>())
        {
            mat.enableInstancing = true;
        }
    }
    
    [MenuItem("Custom/GPUMaterialUninstancing")]
    public static void GPUMaterialUninstancing()
    {
        foreach (var mat in Resources.FindObjectsOfTypeAll<Material>())
        {
            mat.enableInstancing = false;
        }
    }
}
