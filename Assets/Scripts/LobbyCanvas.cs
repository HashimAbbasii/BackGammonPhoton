using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;

public class LobbyCanvas : MonoBehaviour
{
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

}
