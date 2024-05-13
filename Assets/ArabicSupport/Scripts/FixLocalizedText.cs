using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

public class FixLocalizedText : MonoBehaviour
{
    public string text;
    public bool tashkeel = true;
    public bool hinduNumbers = true;

    public TMP_Text showText;
	
    // Use this for initialization
    private void Awake ()
    {
        showText = GetComponent<TMP_Text>();
    }
	
    public void FixText()
    {
        showText.text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
    }
}
