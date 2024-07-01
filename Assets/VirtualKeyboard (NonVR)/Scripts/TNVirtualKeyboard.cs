using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TNVirtualKeyboard : MonoBehaviour
{
	
	public static TNVirtualKeyboard Instance;
	
	public string words = "";
	
	public GameObject vkCanvas;
	
	public InputField targetText;
	public InputField targetText2;
	//public InputField targetText2;

	
	
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
		HideVirtualKeyboard();
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void KeyPress(string k){
		words += k;
		targetText.text = words;
		
		targetText2.text = words;
	}
	
	public void Del(){
		words = words.Remove(words.Length - 1, 1);
		targetText.text = words;
		targetText2.text = words;
	}
	
	public void ShowVirtualKeyboard(){
		vkCanvas.SetActive(true);
	}
	
	public void HideVirtualKeyboard(){
		vkCanvas.SetActive(false);
	}
}
