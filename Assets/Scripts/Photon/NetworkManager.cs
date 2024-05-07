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
    private bool isConnecting = false;
    //public GameObject connectionPanel;
    public static NetworkManager Instance;
    //public void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //    DontDestroyOnLoad(gameObject);
    //}
    
    #endregion
    
    public bool IsConnectedToPhotonServer { get; set; }
    public bool TryToConnectToPhotonServer { get; set; }

    // public SceneField offlineScene;
    // public SceneField onlineScene;
    //public GameObject lobbyCanvas;

    // public GameManager gameManager;
    public PhotonRoomManager photonRoomManager;
    
    private void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
    }


    public void ConnectionEstablish()
    {
        if (!isConnecting)
        {
            isConnecting = true;
           LobbySceneManager.Instance.connectionPanel.SetActive(true); // Show the connection panel

            // Attempt to connect to Photon Server
            PhotonNetwork.ConnectUsingSettings();

            // Schedule a check after 3 seconds to see if the user is connected to the master server
            Invoke(nameof(CheckIsUserConnectedToMasterServer), 3f);
        }
    }
    private void CheckIsUserConnectedToMasterServer()
    {
        // Check if the user is connected to the master server
        if (PhotonNetwork.IsConnected)
        {
            // If connected, hide the connection panel
            LobbySceneManager.Instance.connectionPanel.SetActive(false);
            isConnecting = false;
           photonRoomManager.JoinRandomRoom();
          //StartCoroutine(CreatePlayer());
            // NetworkManager.Instance.photonRoomManager.JoinRandomRoom();


            // photonRoomManager.IN RoomJoined()
        }
        else
        {
            // If not connected, show a message or handle it as needed
            Debug.LogWarning("Failed to connect to Photon Server.");
        }
    }

    //private void Update()
    //{
    //    if (!TryToConnectToPhotonServer)
    //    {
    //        PhotonNetwork.ConnectUsingSettings();
    //        TryToConnectToPhotonServer = true;
    //        Invoke(nameof(CheckIsUserConnectedToMasterServer), 3f);
    //    }

    //}

    //private void CheckIsUserConnectedToMasterServer()
    //{
    //    if (!PhotonNetwork.IsConnected)
    //        IsConnectedToPhotonServer = false;

    //    if (!IsConnectedToPhotonServer)
    //        TryToConnectToPhotonServer = false;
    //}
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player is connected to " + PhotonNetwork.CloudRegion + " server");
        Debug.Log("Player Ping is " + PhotonNetwork.GetPing() );
        LobbySceneManager.Instance.lobbyCanvas.SetActive(true);
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

    public override void OnJoinedRoom()
    {
        StartCoroutine(CreatePlayer());
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
    IEnumerator CreatePlayer()
    {
        // Instantiate player object across the network
        yield return new WaitForSecondsRealtime(2.5f);
        Debug.Log("Instantiating player on: " + PhotonNetwork.IsMasterClient);
        PhotonNetwork.Instantiate("NetworkPlayer", Vector3.zero, Quaternion.identity);
    }

}