using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;
using UnityEngine.UI;
using Photon.Pun;
using Assets.SimpleLocalization.Scripts;
using BackgammonNet.Core;
using System.Runtime.InteropServices;
using TMPro;

public class LobbyCanvas : MonoBehaviourPunCallbacks
{
    [Header("WebGL TopBars")]
    public GameObject onlinePlay;
    public GameObject createRoom;
    public GameObject findRoom;
    public GameObject difficultyLevel;
    public GameObject DifficultyText;
    public GameObject LobbyPanelText;
    public GameObject CreateRoomText;
    public GameObject FindRoomText;




    [Header("WebGL ScalingElements")]
    public GameObject RoomListSearchBar;
    public GameObject RoomListScrollView;

    public GameObject PlayBtnInsideRoomPanel;
    public GameObject keyboardPanel;


    public bool OnPointerDownBool = false;

    [Header("BottomMenuButtons")]
    public Button BsoundBtn;
    public Button BmusicBtn;

    [Header("BottomMenuGameObject")]
    public GameObject BottomMenu;
    public GameObject buttonsPanel;

    [Header("BottomButtonSprites")]
    public Sprite BOnSoundToggle;
    public Sprite BOffSoundToggle;
    public Sprite BOnMusicToggle;
    public Sprite BOffMusicToggle;





    [Header("TestTexts")]
    public TextMeshProUGUI testtext;
    public TextMeshProUGUI testtext2;
    public TextMeshProUGUI testtext3;
    public TMP_Dropdown dropdown;



    [Header("WebGL ScalingElements")]
    public GameObject LoadingPanel;
    public GameObject keysParent;
    public Image keyboardPanelImg;
    public Image keyboardPanelImg2;
    public GameObject keyboardCanvas;



    public Button loginButton;
    public string difficulty;

    [Header("Bools")]
    public bool fullscreenToggle = false;
    public bool soundToggle = false;
    public bool musicToggle = false;
    private bool isToggle = true;

    [Header("Animators")]
    public Animator FullScreenToggleAnimator;
    public Animator VfxSoundToggleAnimator;
    public Animator musicSoundToggleAnimator;

    [Header("ButtonSprites")]
    public Sprite OnSoundToggle;
    public Sprite OffSoundToggle;
    public Sprite OnMusicToggle;
    public Sprite OffMusicToggle;
    public Sprite OnFullScreenToggle;
    public Sprite OffFullScreenToggle;

    [Header("TopMenuButtons")]
    public Button soundBtn;
    public Button musicBtn;
    public Button fullScreenBtn;

    public FullscreenWebGLManager fullScreenWebGLManager;


    public LocalizedTextTMP textText;


    public static LobbyCanvas Instance { get; set; }


#if UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern void closewindow();

   

    public void QuitAndClose()
    {
        Application.Quit();
        closewindow();
    }

#endif

    public void OnInputFieldClicked()
    {
        OnPointerDownBool = true;
    }


    private void Awake()
    {
        Instance = this;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (IsRunningOnAndroid())
            {
                fullScreenWebGLManager.EnterFullscreen();
            }
        }


        AudioManager.Instance.audioMixer.GetFloat("Music", out var mVal);
        AudioManager.Instance.audioMixer.GetFloat("VFX", out var vVal);

        if (mVal > -40)
        {
            //Music is On
            musicBtn.image.sprite = OnMusicToggle;
            BmusicBtn.image.sprite = BOnMusicToggle;
            musicToggle = true;


        }
        else
        {
            //Music is off
            musicBtn.image.sprite = OffMusicToggle;
            BmusicBtn.image.sprite = BOffMusicToggle;
            musicToggle = false;


        }

        if (vVal > -40)
        {

            soundBtn.image.sprite = OnSoundToggle;
            BsoundBtn.image.sprite = BOnSoundToggle;
            soundToggle = true;

        }
        else
        {
            soundBtn.image.sprite = OffSoundToggle;
            BsoundBtn.image.sprite = BOffSoundToggle;
            soundToggle = false;

        }

    }

    private void Update()
    {
        StartCoroutine(RotateCameraAndCanvas());
    }


    public IEnumerator RotateCameraAndCanvas()
    {
        float ratio = (Screen.width * 1f / Screen.height);
        testtext.text = ratio.ToString();

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            var width = Screen.width;
            var height = Screen.height;

            if (width / height < 1)
            {
                ratio = (Screen.width * 1f / Screen.height);
            }




#if UNITY_WEBGL

          




            if (IsRunningOnAndroid() || IsRunningOniOS())
            {
                //  keyboardCanvas.gameObject.SetActive(true);

                BottomMenu.gameObject.SetActive(true);
                buttonsPanel.gameObject.SetActive(false);

                onlinePlay.gameObject.SetActive(true);
                createRoom.gameObject.SetActive(true);
                findRoom.gameObject.SetActive(true);
                difficultyLevel.gameObject.SetActive(true);
                DifficultyText.gameObject.SetActive(true);


                if (ratio < 1)  //_ratio  Portrait Android/iOS
                {


                    RectTransform RoomListSearchBarRectTransform = RoomListSearchBar.GetComponent<RectTransform>();
            RoomListSearchBarRectTransform.anchoredPosition = new Vector2(-1.4314f, 824f);
            RoomListSearchBarRectTransform.sizeDelta = new Vector2(905.1232f, 54.3284f);
            RoomListSearchBarRectTransform.localScale = new Vector3(1f, 1f, 1f);

            RectTransform RoomListScrollViewRectTransform = RoomListScrollView.GetComponent<RectTransform>();
            RoomListScrollViewRectTransform.anchoredPosition = new Vector2(-1.4299f, 300.8922f);
            RoomListScrollViewRectTransform.sizeDelta = new Vector2(905.12f, 943.9157f);
            RoomListScrollViewRectTransform.localScale = new Vector3(1f, 1f, 1f);




                    //RectTransform nextbuttonRectTransform = nextBtn.GetComponent<RectTransform>();         //Next, Random, TimeText Set in Ship Placement Scene
                    //RectTransform randombuttonRectTransform = randomBtn.GetComponent<RectTransform>();
                    //RectTransform enemyShipTextRectTransform = enemyShipText.GetComponent<RectTransform>();
                    //RectTransform playerShipRectTransform = playerShipText.GetComponent<RectTransform>();
                    //RectTransform topTextRectTransform = topTextLocalization.GetComponent<RectTransform>();

                    //nextbuttonRectTransform.anchoredPosition = new Vector3(-137f, 225f, 0f);
                    //randombuttonRectTransform.anchoredPosition = new Vector3(134f, 225f, 0f);

                    testtext2.text = " Android | iOS, Portrait ";


                    RectTransform keysParentRectTransform = keysParent.GetComponent<RectTransform>();
                    keysParentRectTransform.sizeDelta = new Vector3(100f, 100f);
                    keysParentRectTransform.localScale = new Vector3(1.06f, 1.42f, 0.71715f);
                    keysParentRectTransform.localPosition = new Vector2(0.70713f, 110f);


                    RectTransform loadingPanelRectTransform = LoadingPanel.GetComponent<RectTransform>();
                    loadingPanelRectTransform.sizeDelta = new Vector3(1283f, 912.1758f);

                    keyboardPanelImg.gameObject.SetActive(false);
                    keyboardPanelImg2.gameObject.SetActive(false);





                    // Botttom Menu Buttons

                    RectTransform BsoundBtnRectTransform = BsoundBtn.GetComponent<RectTransform>();
                    BsoundBtnRectTransform.sizeDelta = new Vector2(80f, 80f);


                    RectTransform BmusicBtnRectTransform = BmusicBtn.GetComponent<RectTransform>();
                    BmusicBtnRectTransform.sizeDelta = new Vector2(80f, 80f);


                    RectTransform playBtnInsideRoomPanelRectTransform = PlayBtnInsideRoomPanel.GetComponent<RectTransform>();
                    playBtnInsideRoomPanelRectTransform.anchoredPosition = new Vector2(0f, 424f);
                    playBtnInsideRoomPanelRectTransform.sizeDelta = new Vector3(300f, 85f);
                    playBtnInsideRoomPanelRectTransform.localScale = new Vector3(1f, 1f, 1f);


                    RectTransform keyboardPanelRectTransform = keyboardPanel.GetComponent<RectTransform>();
                    keyboardPanelRectTransform.offsetMin = new Vector2(0, keyboardPanelRectTransform.offsetMin.y);
                    keyboardPanelRectTransform.offsetMax = new Vector2(0, keyboardPanelRectTransform.offsetMax.y);
                    Vector2 sizeDelta = keyboardPanelRectTransform.sizeDelta;
                    sizeDelta.y = 201.9893f;
                    keyboardPanelRectTransform.sizeDelta = sizeDelta;
                    Vector2 anchoredPosition = keyboardPanelRectTransform.anchoredPosition;
                    anchoredPosition.y = 145f;
                    keyboardPanelRectTransform.anchoredPosition = anchoredPosition;



                    RectTransform difficultyTextRectTransform = DifficultyText.GetComponent<RectTransform>();
                    difficultyTextRectTransform.anchoredPosition = new Vector2(0.611f, -37f);

                    RectTransform lobbyPanelTextRectTransform = LobbyPanelText.GetComponent<RectTransform>();
                    lobbyPanelTextRectTransform.anchoredPosition = new Vector2(0f, 940f);                 ///////////////////////////

                    RectTransform createRoomTextRectTransform = CreateRoomText.GetComponent<RectTransform>();
                    createRoomTextRectTransform.anchoredPosition = new Vector2(179f, -55f);

                    RectTransform findRoomTextRectTransform = FindRoomText.GetComponent<RectTransform>();
                    findRoomTextRectTransform.anchoredPosition = new Vector2(134f, -63f);



                    //playerShipRectTransform.anchoredPosition = new Vector3(-622f, -476f, 0f);     // Positions for WebGl portrait Ship Placement Scene
                    //playerShipRectTransform.sizeDelta = new Vector2(365f, 70f);
                    //enemyShipTextRectTransform.anchoredPosition = new Vector3(777.9999f, -467f, 0f);
                    //enemyShipTextRectTransform.sizeDelta = new Vector2(375f, 70f);
                }
                                                            


                else if( ratio >=2) //_ratio  LAndScape Android/iOS
                {


                    RectTransform RoomListSearchBarRectTransform = RoomListSearchBar.GetComponent<RectTransform>();
                    RoomListSearchBarRectTransform.anchoredPosition = new Vector2(-174f, 347f);
                    RoomListSearchBarRectTransform.sizeDelta = new Vector2(739.2109f, 54.3284f);
                    RoomListSearchBarRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform RoomListScrollViewRectTransform = RoomListScrollView.GetComponent<RectTransform>();
                    RoomListScrollViewRectTransform.anchoredPosition = new Vector2(-3f, -46f);
                    RoomListScrollViewRectTransform.sizeDelta = new Vector2(1080f, 687.7f);
                    RoomListScrollViewRectTransform.localScale = new Vector3(1f, 1f, 1f);



                    testtext2.text = " Android | iOS, LandScape ";

                    RectTransform keysParentRectTransform = keysParent.GetComponent<RectTransform>();
                    keysParentRectTransform.sizeDelta = new Vector3(100f, 100f);
                    keysParentRectTransform.localScale = new Vector3(0.71715f, 0.71715f, 0.71715f);
                    keysParentRectTransform.localPosition = new Vector2(0.70713f, -0.14143f);


                    RectTransform loadingPanelRectTransform = LoadingPanel.GetComponent<RectTransform>();
                    loadingPanelRectTransform.sizeDelta = new Vector3(1949.1f,1385.721f);

                    if(OnPointerDownBool)
                    {
                        keyboardPanelImg.gameObject.SetActive(true);
                        keyboardPanelImg2.gameObject.SetActive(true);
                    }



                    // Botttom Menu Buttons

                    RectTransform BsoundBtnRectTransform = BsoundBtn.GetComponent<RectTransform>();
                    BsoundBtnRectTransform.sizeDelta = new Vector2(80f, 80f);


                    RectTransform BmusicBtnRectTransform = BmusicBtn.GetComponent<RectTransform>();
                    BmusicBtnRectTransform.sizeDelta = new Vector2(80f, 80f);


                    RectTransform playBtnInsideRoomPanelRectTransform = PlayBtnInsideRoomPanel.GetComponent<RectTransform>();
                    playBtnInsideRoomPanelRectTransform.anchoredPosition = new Vector2(0f, 80f);
                    playBtnInsideRoomPanelRectTransform.sizeDelta = new Vector3(300f, 85f);
                    playBtnInsideRoomPanelRectTransform.localScale = new Vector3(0.796f, 0.796f, 0.796f);



                    if(RoomSearch.roomListActive)
                    {
                        RectTransform keyboardPanelRectTransform = keyboardPanel.GetComponent<RectTransform>();
                        keyboardPanelRectTransform.offsetMin = new Vector2(0, keyboardPanelRectTransform.offsetMin.y);
                        keyboardPanelRectTransform.offsetMax = new Vector2(0, keyboardPanelRectTransform.offsetMax.y);
                        Vector2 sizeDelta = keyboardPanelRectTransform.sizeDelta;
                        sizeDelta.y = 188.07f;
                        keyboardPanelRectTransform.sizeDelta = sizeDelta;
                        Vector2 anchoredPosition = keyboardPanelRectTransform.anchoredPosition;
                        anchoredPosition.y = 120f;
                        keyboardPanelRectTransform.anchoredPosition = anchoredPosition;
                    }
                    else
                    {

                        RectTransform keyboardPanelRectTransform = keyboardPanel.GetComponent<RectTransform>();
                        keyboardPanelRectTransform.offsetMin = new Vector2(0, keyboardPanelRectTransform.offsetMin.y);
                        keyboardPanelRectTransform.offsetMax = new Vector2(0, keyboardPanelRectTransform.offsetMax.y);
                        Vector2 sizeDelta = keyboardPanelRectTransform.sizeDelta;
                        sizeDelta.y = 144.064f;
                        keyboardPanelRectTransform.sizeDelta = sizeDelta;
                        Vector2 anchoredPosition = keyboardPanelRectTransform.anchoredPosition;
                        anchoredPosition.y = 116.0374f;
                        keyboardPanelRectTransform.anchoredPosition = anchoredPosition;
                    }


                    RectTransform difficultyTextRectTransform = DifficultyText.GetComponent<RectTransform>();
                    difficultyTextRectTransform.anchoredPosition = new Vector2(0.611f, -31f);

                    RectTransform lobbyPanelTextRectTransform = LobbyPanelText.GetComponent<RectTransform>();
                    lobbyPanelTextRectTransform.anchoredPosition = new Vector2(0f, 363f);

                    RectTransform createRoomTextRectTransform = CreateRoomText.GetComponent<RectTransform>();
                    createRoomTextRectTransform.anchoredPosition = new Vector2(179f, -55f);

                    RectTransform findRoomTextRectTransform = FindRoomText.GetComponent<RectTransform>();
                    findRoomTextRectTransform.anchoredPosition = new Vector2(134f, -63f);


                }

            }

            

            else if(ratio >= 1.55) // WebGL PC
            {
                BottomMenu.gameObject.SetActive(false);
                buttonsPanel.gameObject.SetActive(true);


                testtext2.text = "WebGL PC";

                RectTransform keysParentRectTransform = keysParent.GetComponent<RectTransform>();
                keysParentRectTransform.sizeDelta = new Vector3(100f, 100f);
                keysParentRectTransform.localScale = new Vector3(0.71715f, 0.71715f, 0.71715f);
                keysParentRectTransform.localPosition = new Vector2(0.70713f, -0.14143f);


                RectTransform loadingPanelRectTransform = LoadingPanel.GetComponent<RectTransform>();
                loadingPanelRectTransform.sizeDelta = new Vector3(1949.1f, 1385.721f);

                keyboardCanvas.gameObject.SetActive(false);


                
                


            }

#endif

        }


        //else
        //{
        //    ratio = (Screen.width * 1f / Screen.height);
        //    var width = Screen.width;
        //    var height = Screen.height;

        //    if (width / height < 1)
        //    {
        //        ratio = (Screen.width * 1f / Screen.height);
        //        ratio = 1 / ratio;
        //    }

        //    float _height = (142.147f * (ratio * ratio)) - (761.729f * ratio) + 2054.87f;
        //}


        yield return null;
    }



    public bool IsRunningOnAndroid()
    {
        return SystemInfo.operatingSystem.ToLower().Contains("android");

    }

    public bool IsRunningOniOS()
    {
        return SystemInfo.operatingSystem.ToLower().Contains("iphone") ||
               SystemInfo.operatingSystem.ToLower().Contains("ipad") ||
               SystemInfo.operatingSystem.ToLower().Contains("ios");
    }

    private void Start()
    {

        MyGameManager.Instance.languageManager.LanguageDropdown = dropdown;

        // LanguageEnum = LanguageEnumerated.German;


        int currentIndex = MyGameManager.Instance.languageManager.testIndex;
        MyGameManager.Instance.languageManager.LanguageSelect(currentIndex);

#if UNITY_ANDROID
        Application.targetFrameRate = 60;
        fullScreenBtn.gameObject.SetActive(false);
#endif
    }


    public void SelectPawn(int pawnSelect)
    {
        MyGameManager.Instance.SelectPawn(pawnSelect);
    }

    public void CreateHostRequest()
    {
        LobbyManager.Instance.CreateHostRequest();
    }

    public void StartGame()
    {
        LobbyManager.Instance.StartGame();
        AudioManager.Instance.PlayGameMusic();


    }

    public void StartGameAi()
    {
        LobbyManager.Instance.StartGameAi();
        AudioManager.Instance.PlayGameMusic();
    }

    public void JoinRandomRoom()
    {
        PhotonRoomManager.Instance.JoinRandomRoom();
    }

    public void OnLoginClick()
    {
        MyPhotonManager.instance.OnLoginClick();
    }

    public void ConnectToHost()
    {
        LobbyManager.Instance.ConnectToHost();
    }

    public void GoToMainMenu()
    {
        LobbyManager.Instance.GoToMainMenu();
    }

    public void ActiveMyPanel(string panelName)
    {
        MyPhotonManager.instance.ActiveMyPanel(panelName);
    }

    public void RoomListBtnClicked()
    {
        MyPhotonManager.instance.RoomListBtnClicked();
    }

    public void OnClickRoomCreate()
    {
        MyPhotonManager.instance.OnClickRoomCreate();
    }

    public void onCancelClick()
    {
        MyPhotonManager.instance.onCancelClick();

        MyPhotonManager.instance.userNameText.text = "";
        MyPhotonManager.instance.roomNameText.text = "";
        TNVirtualKeyboard.Instance.words = "";
    }

    public void BackFromRoomList()
    {
        MyPhotonManager.instance.BackFromRoomList();
    }

    public void BackFromPlayerList()
    {
        MyPhotonManager.instance.BackFromPlayerList();
    }

    public void OnClickPlayButton()
    {
        MyPhotonManager.instance.OnClickPlayButton();
        AudioManager.Instance.PlayGameMusic();
    }

    public void CheckLoginName(string playerName)
    {
        loginButton.interactable = !string.IsNullOrEmpty(playerName);
    }

    public void DisconnectAndReturn()
    {
        Debug.Log("Test");
        PhotonNetwork.Disconnect();
    }

    public void ButtonClicked()
    {
        AudioManager.Instance.ButtonClicked();
    }

    public void ToggleBoolSound()
    {
        Debug.Log("ToggleBoolSound");
        soundToggle = !soundToggle;

        AudioManager.Instance.ToggleVFXSound(soundToggle);

        if (soundToggle)
        {
            Debug.Log("false");
            // VfxSoundToggleAnimator.Play("vfx sound Anim");
            soundBtn.image.sprite = OnSoundToggle;
            BsoundBtn.image.sprite = BOnSoundToggle;
        }
        else
        {
            Debug.Log("true");
            // VfxSoundToggleAnimator.Play("vfx sound Anim Reverse");
            soundBtn.image.sprite = OffSoundToggle;
            BsoundBtn.image.sprite = BOffSoundToggle;
        }
    }

    public void ToggleBoolMusic()
    {
        Debug.Log("ToggleBoolSound");
        musicToggle = !musicToggle;

        AudioManager.Instance.ToggleMusicSound(musicToggle);

        if (musicToggle)
        {
            Debug.Log("false");
            //musicSoundToggleAnimator.Play("Music Anim");
            musicBtn.image.sprite = OnMusicToggle;
            BmusicBtn.image.sprite = BOnMusicToggle;
        }
        else
        {
            Debug.Log("true");
            //musicSoundToggleAnimator.Play("Music Anim Reverse");
            musicBtn.image.sprite = OffMusicToggle;
            BmusicBtn.image.sprite = BOffMusicToggle;
        }

    }


    public void OnPointerDown()
    {
        ToggleBoolean();
        ToggleBoolFullScreen(isToggle);
    }

    public void ToggleBoolean()
    {
        isToggle = !isToggle;
    }

    public void ToggleBoolFullScreen(bool isToggle)
    {
      //  fullscreenToggle = !fullscreenToggle;

        if (!isToggle)
        {
            MyGameManager.Instance.fullScreenWebGLManager.EnterFullscreen();

            //FullScreenToggleAnimator.Play("Full Screen Anim");
            fullScreenBtn.image.sprite = OnFullScreenToggle;
        }
        else
        {
            MyGameManager.Instance.fullScreenWebGLManager.ExitFullscreen();

            //FullScreenToggleAnimator.Play("Full Screen Anim Reverse");
            fullScreenBtn.image.sprite = OffFullScreenToggle;
        }
    }

    public void LanguageChange(int index)
    {
        MyGameManager.Instance.languageManager.LanguageSelect(index);
    }

    public void BeginnerDifficulty()
    {
        MyGameManager.Instance.botDifficulty = Difficulty.Beginner;

        difficulty = "Text.Beginner";
        //GameController.Instance.difficultyTextPausePanel.variableText = difficulty.ToString();
        //LanguageManager.OnVariableChanged();

    }
    public void IntermediateDifficulty()
    {
        MyGameManager.Instance.botDifficulty = Difficulty.Intermediate;
        difficulty = "Text.Intermediate";
        //GameController.Instance.difficultyTextPausePanel.variableText = difficulty.ToString();
       // LanguageManager.OnVariableChanged();

    }
    public void GrandMasterDifficulty()
    {
        MyGameManager.Instance.botDifficulty = Difficulty.GrandMaster;
        difficulty = "Text.GrandMaster";
        //GameController.Instance.difficultyTextPausePanel.variableText = difficulty.ToString();
        //LanguageManager.OnVariableChanged();
    }


}
