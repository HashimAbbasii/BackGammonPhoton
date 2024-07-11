using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FixLocalizedText : MonoBehaviour
{
    public string text;
    public bool tashkeel = true;
    public bool hinduNumbers = true;

    public TMP_Text showText;
    public Text showTextLegacy;

    // Use this for initialization
    private void Awake ()
    {
        if (GetComponent<TMP_Text>())
        {
            showText = GetComponent<TMP_Text>();
        }
        else if (GetComponent<Text>())
        {
            showTextLegacy = GetComponent<Text>();
        }
    }
	
    public void FixText()
    {
        if (showText != null)
        {
            showText.text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
        }
        else if (showTextLegacy != null)
        {
            showTextLegacy.text = ArabicFixer.Fix(text, tashkeel, hinduNumbers);
        }
    }
}
