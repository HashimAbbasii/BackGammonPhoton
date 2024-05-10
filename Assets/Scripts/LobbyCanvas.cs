using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyCanvas : MonoBehaviourPunCallbacks
{
    public Button loginButton;

    [Header("Bools")]
    public bool fullscreenToggle;
    public bool soundToggle;
    public bool musicToggle;

    [Header("Animators")]
    public Animator FullScreenToggleAnimator;
    public Animator VfxSoundToggleAnimator;
    public Animator musicSoundToggleAnimator;


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


    public void ToggleBoolSound(bool toggle)
    {
        soundToggle = toggle;

        AudioManager.Instance.ToggleVFXSound(soundToggle);

        if (toggle)
        {
            VfxSoundToggleAnimator.Play("vfx sound Anim Reverse");
        }
        else
        {
            VfxSoundToggleAnimator.Play("vfx sound Anim");
        }
    }

    public void ToggleBoolMusic(bool toggle)
    {
        musicToggle = toggle;

        AudioManager.Instance.ToggleMusicSound(musicToggle);

        if (toggle)
        {
            musicSoundToggleAnimator.Play("Music Anim Reverse");
        }
        else
        {
            musicSoundToggleAnimator.Play("Music Anim");
        }

    }

    public void ToggleBoolFullScreen(bool toggle)
    {
        fullscreenToggle = toggle;

        if (toggle)
        {
            //MyGameManager.Instance.fullScreenWebGLManager.EnterFullscreen();
            FullScreenToggleAnimator.Play("Full Screen Anim");


        }
        else
        {
            //MyGameManager.Instance.fullScreenWebGLManager.ExitFullscreen();
            FullScreenToggleAnimator.Play("Full Screen Anim Reverse");

        }
    }

    public void LanguageChange(int index)
    {
        MyGameManager.Instance.languageManager.LanguageSelect(index);
    }


}
