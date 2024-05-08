using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

[ExecuteAlways]
public class GlobalFinder : MonoBehaviour
{
    public Vector3 localPosition;
    public Vector3 globalPosition;
    public Quaternion localRotation;
    public Quaternion globalRotation;
    public Vector3 localScale;
    public Vector3 lossyScale;
    
    void Update()
    {
        localPosition = transform.localPosition;
        globalPosition = transform.position;
        localRotation = transform.localRotation;
        globalRotation = transform.rotation;
        localScale = transform.localScale;
        localScale = transform.lossyScale;
    }
}
