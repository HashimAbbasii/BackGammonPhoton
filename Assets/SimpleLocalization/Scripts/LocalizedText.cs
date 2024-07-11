using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleLocalization.Scripts
{
    /// <summary>
    /// Localize text component.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {

        public LocalizedText()
        {
            //Debug.Log(name);
            //_tmpText = GetComponent<TMP_Text>();
        }

        [SerializeField] public TextTypes textType;

        [SerializeField] private Color textColor;

        [SerializeField] private Text _text;
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
            //Debug.Log(_tmpText);

            if (_text == null)
                _text = GetComponent<Text>();
            //Debug.Log(_tmpText);
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
            if (MyGameManager.Instance.languageManager.LanguageEnum == LanguageEnumerated.Arabic)
            {
                //Debug.Log("Arabic");
                if (TryGetComponent<FixLocalizedText>(out var flt))
                {
                    //_text.font = ManagerHandler.Instance.gameManager.languageManager.arabicFont;
                    //_text.fontMaterial = ManagerHandler.Instance.gameManager.languageManager.arabicMaterial;
                    flt.text = LocalizationManager.Localize(localizationKey, variableText);
                    flt.FixText();
                }
            }
            else
            {
                // Debug.Log("else");
                switch (textType)
                {

                    case TextTypes.Heading:
                        //_text.font = MyGameManager.Instance.languageManager.headingFont;
                        //_text.fontMaterial = ManagerHandler.Instance.gameManager.languageManager.headingMaterial;
                        _text.color = textColor;
                        //Debug.Log("1");
                        break;
                    case TextTypes.Button:
                        //_text.font = ManagerHandler.Instance.gameManager.languageManager.buttonFont;
                        //_text.fontMaterial = ManagerHandler.Instance.gameManager.languageManager.buttonMaterial;
                        _text.color = textColor;
                        //Debug.Log("2");
                        break;
                    case TextTypes.Normal:
                        //Debug.Log(_tmpText);
                        //_text.font = ManagerHandler.Instance.gameManager.languageManager.normalFont;
                        //_text.fontMaterial = ManagerHandler.Instance.gameManager.languageManager.normalMaterial;
                        _text.color = textColor;
                        //  Debug.Log("3");
                        break;
                }

                //_tmpText.font = GameManager.Instance.languageManager.normalFont;
                //_tmpText.fontMaterial = GameManager.Instance.languageManager.normalMaterial;
                _text.text = LocalizationManager.Localize(localizationKey, variableText);
            }
        }
    }
}