using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;
using UnityEngine.UI;
using Photon.Pun;
using Assets.SimpleLocalization.Scripts;
using BackgammonNet.Core;

public class LobbyCanvas : MonoBehaviourPunCallbacks
{



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

#if UNITY_ANDROID
        Application.targetFrameRate = 60;
        fullScreenBtn.gameObject.SetActive(false);
#endif
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
            Debug.Log("true");
            // VfxSoundToggleAnimator.Play("vfx sound Anim Reverse");
            soundBtn.image.sprite = OffSoundToggle;
        }
        else
        {
            Debug.Log("false");
            // VfxSoundToggleAnimator.Play("vfx sound Anim");
            soundBtn.image.sprite = OnSoundToggle;
        }
    }

    public void ToggleBoolMusic()
    {
        Debug.Log("ToggleBoolSound");
        musicToggle = !musicToggle;

        AudioManager.Instance.ToggleMusicSound(musicToggle);

        if (musicToggle)
        {
            Debug.Log("true");
            //musicSoundToggleAnimator.Play("Music Anim Reverse");
            musicBtn.image.sprite = OffMusicToggle;
        }
        else
        {
            Debug.Log("false");
            //musicSoundToggleAnimator.Play("Music Anim");
            musicBtn.image.sprite = OnMusicToggle;
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

        if (isToggle)
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
