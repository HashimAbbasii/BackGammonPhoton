using UnityEngine;
using System.Collections;
using ArabicSupport;
using TMPro;
using UnityEngine.UI;

public class SetArabicTextExample : MonoBehaviour {
	
	public string text;
	
	// Use this for initialization
	void Start () {	
		
		if (gameObject.TryGetComponent<Text>(out var _text))
		{
			_text.text = "This sentence (wrong display):\n" + text + "\n\nWill appear correctly as:\n" + ArabicFixer.Fix(text, false, false);
		}
		else if (gameObject.TryGetComponent<TMP_Text>(out var _tmptext))
		{
			_tmptext.text = "This sentence (wrong display):\n" + text + "\n\nWill appear correctly as:\n" + ArabicFixer.Fix(text, false, false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
