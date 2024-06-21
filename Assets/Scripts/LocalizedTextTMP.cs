using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.SimpleLocalization.Scripts
{
    /// <summary>
    /// Localize text component.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextTMP : MonoBehaviour
    {

        public LocalizedTextTMP()
        {
            //Debug.Log(name);
            //_tmpText = GetComponent<TMP_Text>();
        }

        [SerializeField] public TextTypes textType;

        [SerializeField] private Color textColor;

        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private string localizationKey;
        public string variableText;

        public string LocalizationKey
        {
            get => localizationKey;
            set
            {
                localizationKey = value;
                Localize();
            }
        }

        private void Awake()
        {
            if (_tmpText == null)
                _tmpText = GetComponent<TMP_Text>();
        }

        public void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
            LanguageManager.OnVariableChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
            LanguageManager.OnVariableChanged -= Localize;
        }

        [ContextMenu("Localize")]
        public void Localize()
        {
            //Debug.Log("Localize");


            if (MyGameManager.Instance.languageManager.LanguageEnum == LanguageEnumerated.Arabic)
            {
                //Debug.Log("Arabic");
                if (TryGetComponent<FixLocalizedText>(out var flt))
                {
                    _tmpText.font = MyGameManager.Instance.languageManager.arabicFont;
                    _tmpText.fontMaterial = MyGameManager.Instance.languageManager.arabicMaterial;
                    flt.text = LocalizationManager.Localize(localizationKey, variableText);
                    flt.FixText();
                }
            }
            else
            {

                //Debug.Log("else");
                switch (textType)
                {

                    case TextTypes.Heading:
                        _tmpText.font = MyGameManager.Instance.languageManager.headingFont;
                        _tmpText.fontMaterial = MyGameManager.Instance.languageManager.headingMaterial;
                        _tmpText.color = textColor;
                        Debug.Log("1");
                        break;
                    case TextTypes.Button:
                        _tmpText.font = MyGameManager.Instance.languageManager.buttonFont;
                        _tmpText.fontMaterial = MyGameManager.Instance.languageManager.buttonMaterial;
                        _tmpText.color = textColor;
                        Debug.Log("2");
                        break;
                    case TextTypes.Normal:
                        _tmpText.font = MyGameManager.Instance.languageManager.normalFont;
                        _tmpText.fontMaterial = MyGameManager.Instance.languageManager.normalMaterial;
                        _tmpText.color = textColor;
                      //  Debug.Log("3");
                        break;
                }

                //_tmpText.font = GameManager.Instance.languageManager.normalFont;
                //_tmpText.fontMaterial = GameManager.Instance.languageManager.normalMaterial;
                _tmpText.text = LocalizationManager.Localize(localizationKey, variableText);   
            }
        }
    }
}

[Serializable]
public enum TextTypes
{
    Heading,
    Button,
    Normal
}