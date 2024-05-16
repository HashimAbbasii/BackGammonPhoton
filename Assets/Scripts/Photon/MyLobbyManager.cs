using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using WebSocketSharp;
using Random = UnityEngine.Random;

public class MyLobbyManager : MonoBehaviour
{
    [Header("Extras")]
    public string roomName;
    
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject privateCreatePanel;
    public GameObject privateJoinPanel;
    public GameObject randomRoomPanel;
    
    [Header("Buttons")]
    public Button createPrivateGameButton;
    public Button joinPrivateGameButton;
    public Button randomRoomButton;

    private void OnEnable()
    {
        BackToMainPanel();
        createPrivateGameButton.interactable = false;
        joinPrivateGameButton.interactable = false;
        randomRoomButton.interactable = false;
    }

    public void BackToMainPanel()
    {
        mainPanel.SetActive(true);
        privateCreatePanel.SetActive(false);
        privateJoinPanel.SetActive(false);
        randomRoomPanel.SetActive(false);
    }

    public void PrivateCreatePanelOn()
    {
        mainPanel.SetActive(false);
        privateCreatePanel.SetActive(true);
        privateJoinPanel.SetActive(false);
        randomRoomPanel.SetActive(false);
    }
    
    public void PrivateJoinPanelOn()
    {
        mainPanel.SetActive(false);
        privateCreatePanel.SetActive(false);
        privateJoinPanel.SetActive(true);
        randomRoomPanel.SetActive(false);
    }
    
    public void RandomRoomPanelOn()
    {
        mainPanel.SetActive(false);
        privateCreatePanel.SetActive(false);
        privateJoinPanel.SetActive(false);
        randomRoomPanel.SetActive(true);
    }
    
    public void SetRoomName(string rn)
    {
        roomName = rn;
        
        
       // if (!NetworkManager.Instance.photonRoomManager.myUsername.IsNullOrEmpty())
       if (!string.IsNullOrEmpty(NetworkManager.Instance.photonRoomManager.myUsername))
            joinPrivateGameButton.interactable = true;
    }

    public void ResetRoomName()
    {
        roomName = "";
        
        createPrivateGameButton.interactable = false;
        joinPrivateGameButton.interactable = false;
        randomRoomButton.interactable = false;
    }

    public void SetUsername(string un)
    {
        NetworkManager.Instance.photonRoomManager.myUsername = un;

        createPrivateGameButton.interactable = true;
        randomRoomButton.interactable = true;
        // if (!roomName.IsNullOrEmpty())
        if (!string.IsNullOrEmpty(roomName))
            joinPrivateGameButton.interactable = true;
    }

    public void ResetUsername()
    {
        NetworkManager.Instance.photonRoomManager.myUsername = "";
        
        createPrivateGameButton.interactable = false;
        joinPrivateGameButton.interactable = false;
        randomRoomButton.interactable = false;
    }

    public void CreatePrivateRoom()
    {
        roomName = Random.Range(100000, 1000000).ToString();
        NetworkManager.Instance.photonRoomManager.CreatePrivateRoom(roomName);
    }

    public void JoinPrivateRoom()
    {
        NetworkManager.Instance.photonRoomManager.JoinPrivateRoom(roomName);
    }

    public void RandomLobby()
    {
        NetworkManager.Instance.photonRoomManager.JoinRandomRoom();
    }
    
}