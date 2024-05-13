using UnityEngine;
using System.Collections;
using ArabicSupport;
using TMPro;
using UnityEngine.UI;

public class FixGUITextCS : MonoBehaviour {
	
	public string text;
	public bool tashkeel = true;
	public bool hinduNumbers = true;

	public Text showText;
	public TMP_Text showTextTMP;
	
	// Use this for initialization
	private void Start () {
		if (gameObject.TryGetComponent<Text>(out var _text))
		{
			showText = _text;
		}
		else if (gameObject.TryGetComponent<TMP_Text>(out var _tmptext))
		{
			showTextTMP = _tmptext;
		}
	}
	
	// Update is called once per frame
	private void Update () 
	{
		// FixText();
	}

	public void FixText()
	{
		if (showText)
		{
			showText.text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
		}
		else if (showTextTMP)
		{
			showTextTMP.text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
		}
	}
}
