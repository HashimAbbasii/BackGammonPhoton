using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class vkEnabler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ShowVirtualKeyboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void ShowVirtualKeyboard(){
#if UNITY_WEBGL
        TNVirtualKeyboard.Instance.ShowVirtualKeyboard();
		TNVirtualKeyboard.Instance.targetText = gameObject.GetComponent<TMP_InputField>();
#endif
    }
}
