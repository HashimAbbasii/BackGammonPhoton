using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleLocalization.Scripts;
using System.Runtime.InteropServices;
using BackgammonNet.Core;
using TMPro;

public class CanvasHandler : MonoBehaviour
{
    [Header("PlayerObjects")]
    public RectTransform Player0Object;
    public RectTransform Player1Object;

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

    [Header("TopMenuButtons")]
    public Button soundBtn;
    public Button musicBtn;
    public Button fullScreenBtn;

    //public static GameController Instance { get; set; }
    public static CanvasHandler Instance { get; set; }
    public FullscreenWebGLManager fullScreenWebGLManager;

#if UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern void closewindow();



    public void QuitAndClose()
    {
        Application.Quit();
        closewindow();
    }

#endif


    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (IsRunningOnAndroid())
            {
                fullScreenWebGLManager.EnterFullscreen();
            }
        }

        LocalizationManager.Read();


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
            musicToggle = true;

            
        }
        else
        {
            //Music is off
            musicBtn.image.sprite = OffMusicToggle;
            musicToggle = false;

      
        }

        if (vVal > -40)
        {
            
            soundBtn.image.sprite = OnSoundToggle;
            soundToggle = true;
     
        }
        else
        {
            soundBtn.image.sprite = OffSoundToggle;
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

        if(screen_ratio <= 1.5f )
        {
            Player0Object.anchoredPosition = new Vector3(1, 0, 0);
            Player1Object.anchoredPosition = new Vector3(-1, 0, 0);

        }

        else if(screen_ratio > 1.5f)
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

                if (ratio < 1)  //_ratio  Portrait Android/iOS
                {

                    mainCamera.orthographicSize = 20f;

                    //RectTransform buttonsPanelRectTransform = buttonsPanel.GetComponent<RectTransform>();
                   // buttonsPanelRectTransform.localScale = new Vector3(0.49f, 0.49f, 0.49f);

                    RectTransform player0RectTransform = player0.GetComponent<RectTransform>();
                    player0RectTransform.anchoredPosition = new Vector2(0f, 350f);
                    player0RectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);     // 0.52276

                    RectTransform player1RectTransform = player1.GetComponent<RectTransform>();
                    player1RectTransform.anchoredPosition = new Vector2(0f, -300f);                         //-129
                    player1RectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);        //// 0.52276

                    RectTransform submissionPanelRectTransform = submissionPanel.GetComponent<RectTransform>();
                    submissionPanelRectTransform.anchoredPosition = new Vector2(-87f, 164f);
                    submissionPanelRectTransform.localScale = new Vector3(0.7967f, 0.7967f, 0.7967f);

                    RectTransform timerPanelRectTransform = timerPanel.GetComponent<RectTransform>();
                    timerPanelRectTransform.anchoredPosition = new Vector2(0f, 5f);
                    timerPanelRectTransform.localScale = new Vector3(0.79671f, 0.79671f, 0.79671f);


                    RectTransform pauseMenuRectTransform = pauseMenu.GetComponent<RectTransform>();
                    pauseMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform tutorialMenuRectTransform = tutorialMenu.GetComponent<RectTransform>();
                    tutorialMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform gameOverMenuRectTransform = gameOverMenu.GetComponent<RectTransform>();
                    gameOverMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);

                    RectTransform youWinMenuRectTransform = youWinMenu.GetComponent<RectTransform>();
                    youWinMenuRectTransform.localScale = new Vector3(0.49614f, 0.49614f, 0.49614f);


                    RectTransform diceButtonRectTransform = diceButton.GetComponent<RectTransform>();
                    diceButtonRectTransform.anchoredPosition = new Vector2(85f, 5f);
                    diceButtonRectTransform.localScale = new Vector3(0.5761739f, 0.5761739f, 0.5761739f);

                    RectTransform diceResultRectTransform = diceResult.GetComponent<RectTransform>();
                    diceResultRectTransform.anchoredPosition = new Vector2(24f, -300f);
                    diceResultRectTransform.localScale = new Vector3(0.65534f, 0.65534f, 0.65534f);


                }

                else if (ratio >= 2) //_ratio  LAndScape Android/iOS
                {


                    mainCamera.orthographicSize = 8.5f;

                    RectTransform buttonsPanelRectTransform = buttonsPanel.GetComponent<RectTransform>();
                    buttonsPanelRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform player0RectTransform = player0.GetComponent<RectTransform>();
                    player0RectTransform.anchoredPosition = new Vector2(30f, 5.722f);
                    player0RectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);           //1

                    RectTransform player1RectTransform = player1.GetComponent<RectTransform>();
                    player1RectTransform.anchoredPosition = new Vector2(-30f, 7.15f);
                    player1RectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);                //1

                    RectTransform submissionPanelRectTransform = submissionPanel.GetComponent<RectTransform>();
                    submissionPanelRectTransform.anchoredPosition = new Vector2(-16f, 12f);
                    submissionPanelRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform timerPanelRectTransform = timerPanel.GetComponent<RectTransform>();
                    timerPanelRectTransform.anchoredPosition = new Vector2(0f, 5f);
                    timerPanelRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform pauseMenuRectTransform = pauseMenu.GetComponent<RectTransform>();
                    pauseMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform tutorialMenuRectTransform = tutorialMenu.GetComponent<RectTransform>();
                    tutorialMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform gameOverMenuRectTransform = gameOverMenu.GetComponent<RectTransform>();
                    gameOverMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform youWinMenuRectTransform = youWinMenu.GetComponent<RectTransform>();
                    youWinMenuRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform diceButtonRectTransform = diceButton.GetComponent<RectTransform>();
                    diceButtonRectTransform.anchoredPosition = new Vector2(217.5f, 7f);
                    diceButtonRectTransform.localScale = new Vector3(1f, 1f, 1f);

                    RectTransform diceResultRectTransform = diceResult.GetComponent<RectTransform>();
                    diceResultRectTransform.anchoredPosition = new Vector2(87f, -293f);
                    diceResultRectTransform.localScale = new Vector3(1f, 1f, 1f);

                }

            }



            else if (ratio >= 1.55) // WebGL PC
            {


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

        // Debug.Log("ToggleBoolSound");
        soundToggle = !soundToggle;

        AudioManager.Instance.ToggleVFXSound(soundToggle);

        if (soundToggle)
        {
          //  Debug.Log("true");
            // VfxSoundToggleAnimator.Play("vfx sound Anim Reverse");
            soundBtn.image.sprite = OnSoundToggle;
        }
        else
        {
         //   Debug.Log("false");
            // VfxSoundToggleAnimator.Play("vfx sound Anim");
            soundBtn.image.sprite = OffSoundToggle;
        }
    }

    public void ToggleBoolMusic()
    {

        // Debug.Log("ToggleBoolSound");
        musicToggle = !musicToggle;

        AudioManager.Instance.ToggleMusicSound(musicToggle);

        if (musicToggle)
        {
           // Debug.Log("true");
            //musicSoundToggleAnimator.Play("Music Anim Reverse");
            musicBtn.image.sprite = OnMusicToggle;
        }
        else
        {
            Debug.Log("false");
            //musicSoundToggleAnimator.Play("Music Anim");
            musicBtn.image.sprite = OffMusicToggle;
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

    public void ToggleBoolFullScreen(bool istoggle)
    {
       // fullscreenToggle = !fullscreenToggle;

        if (istoggle)
        {

            MyGameManager.Instance.fullScreenWebGLManager.EnterFullscreen();

            //FullScreenR.gameObject.SetActive(true);
            //FullScreenL.gameObject.SetActive(false);

            fullScreenBtn.image.sprite = OnFullScreenToggle;
        }
        else
        {

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
