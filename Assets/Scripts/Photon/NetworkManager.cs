using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton

    public static NetworkManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    #endregion
    
    public bool IsConnectedToPhotonServer { get; set; }
    public bool TryToConnectToPhotonServer { get; set; }

    // public SceneField offlineScene;
    // public SceneField onlineScene;
    public GameObject lobbyCanvas;

    // public GameManager gameManager;
    public PhotonRoomManager photonRoomManager;
    
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (!TryToConnectToPhotonServer)
        {
            PhotonNetwork.ConnectUsingSettings();
            TryToConnectToPhotonServer = true;
            Invoke(nameof(CheckIsUserConnectedToMasterServer), 3f);
        }
        
    }
    
    private void CheckIsUserConnectedToMasterServer()
    {
        if (!PhotonNetwork.IsConnected)
            IsConnectedToPhotonServer = false;

        if (!IsConnectedToPhotonServer)
            TryToConnectToPhotonServer = false;
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player is connected to " + PhotonNetwork.CloudRegion + " server");
        Debug.Log("Player Ping is " + PhotonNetwork.GetPing() );
        lobbyCanvas.SetActive(true);
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        IsConnectedToPhotonServer = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        IsConnectedToPhotonServer = false;
        TryToConnectToPhotonServer = false;
    }
    
    
    
    // public override void OnJoinedLobby()
    // {
    //     lobbyCanvas.SetActive(true);
    // }
    //
    // public override void OnLeftLobby()
    // {
    //     lobbyCanvas.SetActive(false);
    // }
    
    
}