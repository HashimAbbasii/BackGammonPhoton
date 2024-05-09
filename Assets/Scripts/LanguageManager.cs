using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] private LanguageEnumerated _languageEnum;

    [Header ("Fonts")]
    public TMP_FontAsset normalFont;
    public TMP_FontAsset headingFont;
    public TMP_FontAsset buttonFont;
    public TMP_FontAsset arabicFont;

    [Header("Font Materials")]
    public Material normalMaterial;
    public Material headingMaterial;
    public Material buttonMaterial;
    public Material arabicMaterial;

    public TMP_Dropdown LanguageDropdown;
    public List<Sprite> flags;

    public LanguageEnumerated LanguageEnum
    {
        get => _languageEnum;
        set
        {
            _languageEnum = value;
            LocalizationManager.Language = _languageEnum.ToString();
        }
    }

    public static Action OnVariableChanged = () => { };

    private string _languageString;

    private void Start()
    {
        LanguageEnum = _languageEnum;
    }

    private void OnValidate()
    {
        LanguageEnum = _languageEnum;
    }

    public void LanguageSelect(int index)
    {
        switch (index)
        {
            case 0:
                LanguageEnum = LanguageEnumerated.English;
               // LanguageDropdown.image = flags(0);
                LanguageDropdown.GetComponent<Image>().sprite = flags[0];
                break;
            case 1:
                LanguageEnum = LanguageEnumerated.Spanish;
                LanguageDropdown.GetComponent<Image>().sprite = flags[1];
                break;
            case 2:
                LanguageEnum = LanguageEnumerated.Russian;
                LanguageDropdown.GetComponent<Image>().sprite = flags[2];
                break;
            case 3:
                LanguageEnum = LanguageEnumerated.Ukrainian;
                LanguageDropdown.GetComponent<Image>().sprite = flags[3];
                break;
            case 4:
                LanguageEnum = LanguageEnumerated.French;
                LanguageDropdown.GetComponent<Image>().sprite = flags[4];
                break;
            case 5:
                LanguageEnum = LanguageEnumerated.Italian;
                LanguageDropdown.GetComponent<Image>().sprite = flags[5];
                break;
            case 6:
                LanguageEnum = LanguageEnumerated.Arabic;
                LanguageDropdown.GetComponent<Image>().sprite = flags[6];
                break;
            case 7:
                LanguageEnum = LanguageEnumerated.Portuguese;
                LanguageDropdown.GetComponent<Image>().sprite = flags[7];
                break;
            case 8:
                LanguageEnum = LanguageEnumerated.German;
                LanguageDropdown.GetComponent<Image>().sprite = flags[8];
                break;
            
        }
    }
}

[Serializable]
public enum LanguageEnumerated
{
    English,
    Spanish,
    Russian,
    Ukrainian,
    French,
    Italian,
    Arabic,
    Portuguese,
    German
    
}