using UnityEngine;
using UnityEditor; 
using System.IO; 
using System.Reflection;

public class DebugMenu  : ScriptableWizard
{

    public long fileID;
    
    [MenuItem("Custom/DebugMenu")]
    static void CreateWizard()
    {
        DisplayWizard("DebugMenu", typeof(DebugMenu), "Find and Close", "Find");
    }

    void OnWizardOtherButton()
    {
	    GameObject resultGo = GetGameObjectFromFileID(fileID);
	    if (resultGo == null)
	    {
		    Debug.LogError("FileID not found for fileID = " + fileID);
		    return;
	    }

	    Debug.Log("GameObject for fileID " + fileID + " is " + resultGo, resultGo);
	    GameObject[] newSelection = new GameObject[] { resultGo };
	    Selection.objects = newSelection;
    }

    void OnWizardCreate()
    {
	    GameObject resultGo = GetGameObjectFromFileID(fileID);
	    if (resultGo == null)
	    {
		    Debug.LogError("FileID not found for fileID = " + fileID);
		    return;
	    }

	    Debug.Log("GameObject for fileID " + fileID + " is " + resultGo, resultGo);
	    GameObject[] newSelection = new GameObject[] { resultGo };
	    Selection.objects = newSelection;
    }

    private static GameObject GetGameObjectFromFileID(long fileID) // also called local identifier
    {
        GameObject resultGo = null;
        var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        // Test every gameobjects
        foreach (var go in gameObjects)
        {
#if UNITY_EDITOR
		PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
		SerializedObject serializedObject = new SerializedObject(go);
		inspectorModeInfo.SetValue (serializedObject, InspectorMode.Debug, null);
		SerializedProperty localIdProp = serializedObject.FindProperty ("m_LocalIdentfierInFile");
#endif
            if(localIdProp.longValue == fileID) resultGo = go;
        }
        // Test every gameobjects transforms
        foreach (var go in gameObjects)
        {
#if UNITY_EDITOR
		PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
		SerializedObject serializedObject = new SerializedObject(go.transform);
		inspectorModeInfo.SetValue (serializedObject, InspectorMode.Debug, null);
		SerializedProperty localIdProp = serializedObject.FindProperty ("m_LocalIdentfierInFile");
#endif
            if(localIdProp.longValue == fileID) resultGo = go;
        }
        return resultGo;
    }
    
    // [MenuItem("Debug/Show gameobject from fileID")]
    // public static void ShowGameObject()
    // {
    //     // Warning, only working in editor
    //     long fileID = 133239613;
    //     GameObject resultGo = GetGameObjectFromFileID(fileID);
    //     if (resultGo == null)
    //     {
    //         Debug.LogError("FileID not found for fileID = " + fileID);
    //         return;
    //     }
    //     Debug.Log("GameObject for fileID " + fileID + " is " + resultGo, resultGo);
    //     GameObject[] newSelection = new GameObject[]{resultGo};
    //     Selection.objects = newSelection;
    // }
}