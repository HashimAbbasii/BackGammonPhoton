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
        PhotonManager.Instance.OnLoginClick();
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
        PhotonManager.Instance.ActiveMyPanel(panelName);
    }

    public void RoomListBtnClicked()
    {
        PhotonManager.Instance.RoomListBtnClicked();
    }

    public void OnClickRoomCreate()
    {
        PhotonManager.Instance.OnClickRoomCreate();
    }

    public void onCancelClick()
    {
        PhotonManager.Instance.onCancelClick();
    }

    public void BackFromRoomList()
    {
        PhotonManager.Instance.BackFromRoomList();
    }

    public void BackFromPlayerList()
    {
        PhotonManager.Instance.BackFromPlayerList();
    }

    public void OnClickPlayButton()
    {
        PhotonManager.Instance.OnClickPlayButton();
    }

}
