using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DestroyOffObjects : MonoBehaviour
{
    [MenuItem("Custom/Delete Selected Objects %J")]
        public static void Ground()
        {
            foreach(var obj in Selection.gameObjects){
                if (!obj.activeSelf)
                {
                    Debug.Log("Destroying " + obj.name);
                    // DestroyImmediate(obj);
                }
            }
        }
}
