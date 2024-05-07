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
        photonManager.Instance.OnLoginClick();
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
        photonManager.Instance.ActiveMyPanel(panelName);
    }

    public void RoomListBtnClicked()
    {
        photonManager.Instance.RoomListBtnClicked();
    }

    public void OnClickRoomCreate()
    {
        photonManager.Instance.OnClickRoomCreate();
    }

    public void onCancelClick()
    {
        photonManager.Instance.onCancelClick();
    }

    public void BackFromRoomList()
    {
        photonManager.Instance.BackFromRoomList();
    }

    public void BackFromPlayerList()
    {
        photonManager.Instance.BackFromPlayerList();
    }

    public void OnClickPlayButton()
    {
        photonManager.Instance.OnClickPlayButton();
    }

}
