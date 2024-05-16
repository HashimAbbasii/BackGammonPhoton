using UnityEditor;
using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    [MenuItem("Custom/Snap To Ground %G")]
    public static void Ground()
    {
        foreach(var transform in Selection.transforms)
        {
            var hitsDown = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 1000f);

            foreach(var hit in hitsDown)
            {
                Debug.Log(hit.collider);
                if (hit.collider.gameObject == transform.gameObject)
                    continue;
            
                transform.position = hit.point;
                break;
            }
            
        }
    }

    
    [MenuItem("Custom/Snap To Ground and Rotate #%&G")]
    public static void GroundAndRotate()
    {
        foreach(var transform in Selection.transforms)
        {
            var hitsDown = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 1000f);

            foreach(var hit in hitsDown)
            {
                Debug.Log(hit.collider);
                if (hit.collider.gameObject == transform.gameObject)
                    continue;
            
                transform.position = hit.point;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, hit.normal);
                break;
            }
            
        }
    }
}
