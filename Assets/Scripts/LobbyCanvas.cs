using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;
using UnityEngine.UI;
using Photon.Pun;
using Assets.SimpleLocalization.Scripts;

public class LobbyCanvas : MonoBehaviourPunCallbacks
{



    public Button loginButton;

    [Header("Bools")]
    public bool fullscreenToggle = false;
    public bool soundToggle = false;
    public bool musicToggle = false;

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


    public LocalizedTextTMP textText;


    private void Start()
    {
        textText.variableText = "Hello Hello";
        LanguageManager.OnVariableChanged();

    }


    public void CreateHostRequest()
    {
        LobbyManager.Instance.CreateHostRequest();
    }

    public void StartGame()
    {
        LobbyManager.Instance.StartGame();
    }

    public void StartGameAi()
    {
        LobbyManager.Instance.StartGameAi();
    }

    public void JoinRandomRoom()
    {
        PhotonRoomManager.Instance.JoinRandomRoom();
    }

    public void OnLoginClick()
    {
        MyPhotonManager.Instance.OnLoginClick();
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
        MyPhotonManager.Instance.ActiveMyPanel(panelName);
    }

    public void RoomListBtnClicked()
    {
        MyPhotonManager.Instance.RoomListBtnClicked();
    }

    public void OnClickRoomCreate()
    {
        MyPhotonManager.Instance.OnClickRoomCreate();
    }

    public void onCancelClick()
    {
        MyPhotonManager.Instance.onCancelClick();
    }

    public void BackFromRoomList()
    {
        MyPhotonManager.Instance.BackFromRoomList();
    }

    public void BackFromPlayerList()
    {
        MyPhotonManager.Instance.BackFromPlayerList();
    }

    public void OnClickPlayButton()
    {
        MyPhotonManager.Instance.OnClickPlayButton();
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

    public void ToggleBoolFullScreen()
    {
        fullscreenToggle = !fullscreenToggle;

        if (fullscreenToggle)
        {
            //FullScreenToggleAnimator.Play("Full Screen Anim");
            fullScreenBtn.image.sprite = OnFullScreenToggle;
        }
        else
        {
            //FullScreenToggleAnimator.Play("Full Screen Anim Reverse");
            fullScreenBtn.image.sprite = OffFullScreenToggle;
        }
    }

    public void LanguageChange(int index)
    {
        MyGameManager.Instance.languageManager.LanguageSelect(index);
    }


}
