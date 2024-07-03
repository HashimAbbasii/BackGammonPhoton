using Assets.SimpleLocalization.Scripts;
using BackgammonNet.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandlerNetwork : MonoBehaviour
{
    [Header("BottomMenuVerticalLayoutgroup")]
    public HorizontalLayoutGroup bottomMenuVerticalLayoutgroup;

    [Header("BottomMenuGameObject")]
    public GameObject BottomMenu;


    [Header("PlayerObjects")]
    public RectTransform Player0Object;
    public RectTransform Player1Object;

    public GameObject Highlight2P0;
    public GameObject Highlight2P1;

    public RectTransform Points0Object;
    public RectTransform Points1Object;


    public GameObject diceRollButton;
    public GameObject diceResults;

    [Header("WebGL ScalingElements")]
    public Camera mainCamera;
    public GameObject buttonsPanel;
    public GameObject player0;
    public GameObject player1;
    public GameObject submissionPanel;
    public GameObject timerPanel;
    public GameObject pauseMenu;
    public GameObject tutorialMenu;
    public GameObject gameOverMenu;
    public GameObject youWinMenu;
    public GameObject diceButton;
    public GameObject diceResult;



    [Header("Bools")]
    public bool fullscreenToggle = false;
    public bool soundToggle = false;
    public bool musicToggle = false;
    private bool isToggle = true;

    [Header("ButtonSprites")]
    public Sprite OnSoundToggle;
    public Sprite OffSoundToggle;
    public Sprite OnMusicToggle;
    public Sprite OffMusicToggle;
    public Sprite OnFullScreenToggle;
    public Sprite OffFullScreenToggle;

    [Header("BottomButtonSprites")]
    public Sprite BOnSoundToggle;
    public Sprite BOffSoundToggle;
    public Sprite BOnMusicToggle;
    public Sprite BOffMusicToggle;
    public Sprite BOnFullScreenToggle;
    public Sprite BOffFullScreenToggle;



    [Header("TopMenuButtons")]
    public Button soundBtn;
    public Button musicBtn;
    public Button fullScreenBtn;

    [Header("BottomMenuButtons")]
    public Button BsoundBtn;
    public Button BmusicBtn;
    
    public Button BfullScreenBtn;



    public static CanvasHandlerNetwork Instance { get; set; }

    public FullscreenWebGLManager fullScreenWebGLManager;

    private void Start()
    {

        bottomMenuVerticalLayoutgroup.transform.GetChild(0).gameObject.SetActive(true);
        bottomMenuVerticalLayoutgroup.transform.GetChild(1).gameObject.SetActive(true);
        bottomMenuVerticalLayoutgroup.transform.GetChild(2).gameObject.SetActive(true);

        bottomMenuVerticalLayoutgroup.transform.GetChild(3).gameObject.SetActive(false);
        bottomMenuVerticalLayoutgroup.transform.GetChild(4).gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (IsRunningOnAndroid())
            {
                fullScreenWebGLManager.EnterFullscreen();
            }
        }

        //LocalizationManager.Read();


        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

#if UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////  Player 0 , Player 1 adjustment for 600x800



        float screen_ratio = (Screen.width * 1f / Screen.height);

        if (screen_ratio <= 1.5f)
        {
            Player0Object.anchoredPosition = new Vector3(0, 0, 0);
            Player1Object.anchoredPosition = new Vector3(0, 0, 0);

        }

        else if (screen_ratio > 1.5f)
        {
            Player0Object.anchoredPosition = new Vector3(15, 0, 0);
            Player1Object.anchoredPosition = new Vector3(-15, 0, 0);
        }




        ///////////////////////////////////////////////////////////////////////////////////////////////////////////





        // -------------------------------------------------------- Portrait WEBGL Commented Area-------------------------------------------------------------------------------//

        float ratio = (Screen.width * 1f / Screen.height);
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            var width = Screen.width;
            var height = Screen.height;

            if (width / height < 1)
            {
                ratio = (Screen.width * 1f / Screen.height);
            }


            if (IsRunningOnAndroid() || IsRunningOniOS())
            {
                BottomMenu.gameObject.SetActive(true);
                buttonsPanel.gameObject.SetActive(false);


                if (ratio < 1)  //_ratio  Portrait Android/iOS
                {

                    mainCamera.orthographicSize = 20f;
                    mainCamera.transform.localPosition = new Vector3(0f, 0f, -100f);



                    RectTransform player0RectTransform = player0.GetComponent<RectTransform>();
                    player0RectTransform.anchoredPosition = new Vector2(134f, 301f);
                    player0RectTransform.sizeDelta = new Vector2(100f, 600f);
                    player0RectTransform.localScale = new Vector3(1f, 1f, 1f);     // 0.52276

                    RectTransform points0RectTransform = Points0Object.GetComponent<RectTransform>();
                    points0RectTransform.anchoredPosition = new Vector2(253f, 298.331f);
                    points0RectTransform.sizeDelta = new Vector2(100f, 118.29f);
                    points0RectTransform.localScale = new Vector3(1f, 1f, 1f);     // 0.52276


                    RectTransform player1RectTransform = player1.GetComponent<RectTransform>();
                    player1RectTransform.anchoredPosition = new Vector2(-251f, -305f);                         //-129
                    player1RectTransform.sizeDelta = new Vector2(100f, 600f);
                    player1RectTransform.localScale = new Vector3(1f, 1f, 1f);        //// 0.52276

                    RectTransform points1RectTransform = Points1Object.GetComponent<RectTransform>();
                    points1RectTransform.anchoredPosition = new Vector2(-132.1f, -313.8011f);
                    points1RectTransform.sizeDelta = new Vector2(100f, 105.84f);
                    points1RectTransform.localScale = new Vector3(1f, 1f, 1f);     // 0.52276



                    RectTransform submissionPanelRectTransform = submissionPanel.GetComponent<RectTransform>();
                    submissionPanelRectTransform.anchoredPosition = new Vector2(0f, 714f);
                    submissionPanelRectTransform.sizeDelta = new Vector2(266.84f, 56.4f);
                    submissionPanelRectTransform.localScale = new Vector3(0.7967f, 0.7967f, 0.7967f);

                    RectTransform timerPanelRectTransform = timerPanel.GetComponent<RectTransform>();
                    timerPanelRectTransform.anchoredPosition = new Vector2(0f, 111f);
                    timerPanelRectTransform.sizeDelta = new Vector2(132.76f, 50f);
                    timerPanelRectTransform.localScale = new Vector3(0.79671f, 0.79671f, 0.79671f);

                    RectTransform highlight2P0RectTransform = Highlight2P0.GetComponent<RectTransform>();
                    highlight2P0RectTransform.anchoredPosition = new Vector2(0.7928f, -2.1836f);
                    highlight2P0RectTransform.sizeDelta = new Vector2(139.1281f, 142.9118f);
                    highlight2P0RectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform highlight2P1RectTransform = Highlight2P1.GetComponent<RectTransform>();
                    highlight2P1RectTransform.anchoredPosition = new Vector2(0.6516991f, -1.4799f);
                    highlight2P1RectTransform.sizeDelta = new Vector2(135.54f, 124.3453f);
                    highlight2P1RectTransform.localScale = new Vector3(1f, 1f, 1f);





                    RectTransform pauseMenuRectTransform = pauseMenu.GetComponent<RectTransform>();
                    pauseMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform tutorialMenuRectTransform = tutorialMenu.GetComponent<RectTransform>();
                    tutorialMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform gameOverMenuRectTransform = gameOverMenu.GetComponent<RectTransform>();
                    gameOverMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform youWinMenuRectTransform = youWinMenu.GetComponent<RectTransform>();
                    youWinMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);




                

                    RectTransform YouWinPanelportraitRectTransform =  GameControllerNetwork.Instance.YouWinPanelportrait.GetComponent<RectTransform>();
                    YouWinPanelportraitRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform gameOverPanelportraitRectTransform = GameControllerNetwork.Instance.gameOverPanelportrait.GetComponent<RectTransform>();
                    gameOverPanelportraitRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform pausePanelportraitRectTransform = GameControllerNetwork.Instance.pausePanelportrait.GetComponent<RectTransform>();
                    pausePanelportraitRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);






                    RectTransform diceButtonRectTransform = diceButton.GetComponent<RectTransform>();
                    diceButtonRectTransform.anchoredPosition = new Vector2(164f, 5f);
                    diceButtonRectTransform.sizeDelta = new Vector2(98f, 98f);
                    diceButtonRectTransform.localScale = new Vector3(0.5761739f, 0.5761739f, 0.5761739f);

                    RectTransform diceResultRectTransform = diceResult.GetComponent<RectTransform>();
                    diceResultRectTransform.anchoredPosition = new Vector2(66f, -536f);
                    diceResultRectTransform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

                    // Botttom Menu Buttons

                    RectTransform BsoundBtnRectTransform = BsoundBtn.GetComponent<RectTransform>();
                    BsoundBtnRectTransform.sizeDelta = new Vector2(60f, 60f);


                    RectTransform BmusicBtnRectTransform = BmusicBtn.GetComponent<RectTransform>();
                    BmusicBtnRectTransform.sizeDelta = new Vector2(60f,60f);

                    


                    RectTransform BfullScreenBtnRectTransform = BfullScreenBtn.GetComponent<RectTransform>();
                    BfullScreenBtnRectTransform.sizeDelta = new Vector2(60f, 60f);





                }

                else if (ratio >= 2) //_ratio  LAndScape Android/iOS
                {


                    //mainCamera.orthographicSize = 8.5f;
                    mainCamera.orthographicSize = 7.08f;
                    mainCamera.transform.localPosition = new Vector3(0f, -0.48f, -100f);

                    //RectTransform buttonsPanelRectTransform = buttonsPanel.GetComponent<RectTransform>();
                    //buttonsPanelRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform player0RectTransform = player0.GetComponent<RectTransform>();
                    player0RectTransform.anchoredPosition = new Vector2(200f, 165.722f);
                    player0RectTransform.sizeDelta = new Vector2(100f, 600f);
                    player0RectTransform.localScale = new Vector3(2f, 2f, 2f);           //1

                    RectTransform points0RectTransform = Points0Object.GetComponent<RectTransform>();
                    points0RectTransform.anchoredPosition = new Vector2(201f, -57f);
                    points0RectTransform.sizeDelta = new Vector2(100f, 93.6f);
                    points0RectTransform.localScale = new Vector3(2f, 2f, 2f);     // 0.52276




                    RectTransform player1RectTransform = player1.GetComponent<RectTransform>();
                    player1RectTransform.anchoredPosition = new Vector2(-199f, 167.15f);
                    player1RectTransform.sizeDelta = new Vector2(100f, 600f);
                    player1RectTransform.localScale = new Vector3(2f, 2f, 2f);                //1

                    RectTransform points1RectTransform = Points1Object.GetComponent<RectTransform>();
                    points1RectTransform.anchoredPosition = new Vector2(-197, -53f);
                    points1RectTransform.sizeDelta = new Vector2(100f, 93.6026f);
                    points1RectTransform.localScale = new Vector3(2f, 2f, 2f);     // 0.52276




                    RectTransform highlight2P0RectTransform = Highlight2P0.GetComponent<RectTransform>();
                    highlight2P0RectTransform.anchoredPosition = new Vector2(1.1981f, -1.5057f);
                    highlight2P0RectTransform.sizeDelta = new Vector2(129.09f, 110.7972f);
                    highlight2P0RectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform highlight2P1RectTransform = Highlight2P1.GetComponent<RectTransform>();
                    highlight2P1RectTransform.anchoredPosition = new Vector2(-1.011398f, -2.1671f);
                    highlight2P1RectTransform.sizeDelta = new Vector2(135.54f, 108.3795f);
                    highlight2P1RectTransform.localScale = new Vector3(1f, 1f, 1f);





                    RectTransform submissionPanelRectTransform = submissionPanel.GetComponent<RectTransform>();
                    //submissionPanelRectTransform.anchoredPosition = new Vector2(-16f, 12f);
                    submissionPanelRectTransform.anchoredPosition = new Vector2(913f, 93f);
                    submissionPanelRectTransform.sizeDelta = new Vector2(338f, 56f);
                    submissionPanelRectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

                    RectTransform timerPanelRectTransform = timerPanel.GetComponent<RectTransform>();
                    timerPanelRectTransform.anchoredPosition = new Vector2(-891f, 198f);
                    timerPanelRectTransform.sizeDelta = new Vector2(129.5194f, 50f);
                    timerPanelRectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                    RectTransform pauseMenuRectTransform = pauseMenu.GetComponent<RectTransform>();
                    pauseMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform tutorialMenuRectTransform = tutorialMenu.GetComponent<RectTransform>();
                    tutorialMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform gameOverMenuRectTransform = gameOverMenu.GetComponent<RectTransform>();
                    gameOverMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform youWinMenuRectTransform = youWinMenu.GetComponent<RectTransform>();
                    youWinMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);




                    RectTransform YouWinPanelportraitRectTransform = GameControllerNetwork.Instance.YouWinPanelportrait.GetComponent<RectTransform>();
                    YouWinPanelportraitRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform gameOverPanelportraitRectTransform = GameControllerNetwork.Instance.gameOverPanelportrait.GetComponent<RectTransform>();
                    gameOverPanelportraitRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform pausePanelportraitRectTransform = GameControllerNetwork.Instance.pausePanelportrait.GetComponent<RectTransform>();
                    pausePanelportraitRectTransform.localScale = new Vector3(1f, 1f, 1f);




                    RectTransform diceButtonRectTransform = diceButton.GetComponent<RectTransform>();
                    diceButtonRectTransform.anchoredPosition = new Vector2(493f, 24f);
                    diceButtonRectTransform.sizeDelta = new Vector2(98f, 98f);
                    diceButtonRectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                    RectTransform diceResultRectTransform = diceResult.GetComponent<RectTransform>();
                    diceResultRectTransform.anchoredPosition = new Vector2(236f, -528f);
                    diceResultRectTransform.sizeDelta = new Vector2(0f, 0f);
                    diceResultRectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


                    // Botttom Menu Buttons

                    RectTransform BsoundBtnRectTransform = BsoundBtn.GetComponent<RectTransform>();
                    BsoundBtnRectTransform.sizeDelta = new Vector2(70f,70f);


                    RectTransform BmusicBtnRectTransform = BmusicBtn.GetComponent<RectTransform>();
                    BmusicBtnRectTransform.sizeDelta = new Vector2(70f, 70f);

                   


                    RectTransform BfullScreenBtnRectTransform = BfullScreenBtn.GetComponent<RectTransform>();
                    BfullScreenBtnRectTransform.sizeDelta = new Vector2(70f, 70f);

                }

            }



            else if (ratio >= 1.55) // WebGL PC
            {

                BottomMenu.gameObject.SetActive(false);
                buttonsPanel.gameObject.SetActive(true);

                //RectTransform keysParentRectTransform = keysParent.GetComponent<RectTransform>();
                //keysParentRectTransform.sizeDelta = new Vector3(100f, 100f);
                //keysParentRectTransform.localScale = new Vector3(0.71715f, 0.71715f, 0.71715f);
                //keysParentRectTransform.localPosition = new Vector2(0.70713f, -0.14143f);


                //RectTransform loadingPanelRectTransform = LoadingPanel.GetComponent<RectTransform>();
                //loadingPanelRectTransform.sizeDelta = new Vector3(1949.1f, 1385.721f);


            }

        }

        // -------------------------------------------------------- Portrait WEBGL Commented Area -------------------------------------------------------------------------------//



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


    public void ToggleBoolSound()
    {

        Debug.Log("ToggleBoolSound");
        soundToggle = !soundToggle;

        AudioManager.Instance.ToggleVFXSound(soundToggle);

        if (soundToggle)
        {
            Debug.Log("true");
            // VfxSoundToggleAnimator.Play("vfx sound Anim Reverse");
            soundBtn.image.sprite = OnSoundToggle;
            BsoundBtn.image.sprite = BOnSoundToggle;
        }
        else
        {
            Debug.Log("false");
            // VfxSoundToggleAnimator.Play("vfx sound Anim");
            soundBtn.image.sprite = OffSoundToggle;
            BsoundBtn.image.sprite = BOffSoundToggle;
        }
    }

    public void ToggleBoolMusic()
    {

        Debug.Log("ToggleBoolSound");
        musicToggle = !musicToggle;

        AudioManager.Instance.ToggleMusicSound(musicToggle); // !

        if (musicToggle)
        {
            Debug.Log("true");
            //musicSoundToggleAnimator.Play("Music Anim Reverse");
            musicBtn.image.sprite = OnMusicToggle;
            BmusicBtn.image.sprite = BOnMusicToggle;
        }
        else
        {
            Debug.Log("false");
            //musicSoundToggleAnimator.Play("Music Anim");
            musicBtn.image.sprite = OffMusicToggle;
            BmusicBtn.image.sprite = BOffMusicToggle;
        }

    }
    public void OnPointerDown()
    {
        

        ToggleBoolean();
        Debug.Log("Pointer Down" + isToggle);

        ToggleBoolFullScreen(isToggle);
    }

    public void ToggleBoolean()
    {
        isToggle = !isToggle;
    }

    public void ToggleBoolFullScreen(bool istoggle)
    {
        // fullscreenToggle = !fullscreenToggle;

        Debug.Log("ToggleBoolFullScreen" + isToggle);

        if (istoggle)
        {
            Debug.Log("istoggle");
            MyGameManager.Instance.fullScreenWebGLManager.EnterFullscreen();

            //FullScreenR.gameObject.SetActive(true);
            //FullScreenL.gameObject.SetActive(false);

            fullScreenBtn.image.sprite = OnFullScreenToggle;
        }
        else
        {
            Debug.Log("else");

            MyGameManager.Instance.fullScreenWebGLManager.ExitFullscreen();

            //FullScreenR.gameObject.SetActive(false);
            //FullScreenL.gameObject.SetActive(true);

            fullScreenBtn.image.sprite = OffFullScreenToggle;
        }
    }

    public void ButtonClicked()
    {
        AudioManager.Instance.ButtonClicked();
    }



}
