using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public string myUsername;

    public RoomType roomType = RoomType.None;
    public string roomName;
    // public TMP_Text region;

    private PhotonView _photonView;

    // public GameObject myPlayer;
    
    // public List<GameObject> playerObjects = new List<GameObject>();

    // public static NetworkManager _instance;

    public delegate void JoinRoom();
    public static event JoinRoom OnRoomJoined;

    public event Action NoRoomAvailable;
    
    private void Start()
    {
        Debug.Log(PhotonNetwork.LocalPlayer);
        NetworkManager.Instance.photonRoomManager = this;
    }
    
    #region Public Room

    //create a public room
    public void CreateOrJoinPublicRoom(string roomKey)
    {
        roomType = RoomType.Public;
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.JoinOrCreateRoom(roomKey, roomOptions, TypedLobby.Default);
        Debug.Log("Creating or Joining room");
    }

    //create a public room
    public void CreatePublicRoom(string roomKey)
    {
        roomType = RoomType.Public;
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomKey, roomOptions);
    }

    //join a specific public room
    public void JoinPublicRoom(string roomKey)
    {
        roomType = RoomType.Public;
        PhotonNetwork.JoinRoom(roomKey);
        Debug.Log("Joining room");
    }

    //join a public room randomly
    public void JoinRandomRoom()
    {
        MyGameManager.Instance.isMultiplayer = true;
        roomType = RoomType.Public;
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Join Room");
       // StartCoroutine(NetworkManager.Instance.CreatePlayer());
        
    }
    
    //no random room is available
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //NoRoomAvailable?.Invoke();
        HostRandom();
        // Here
    }

    
    
    
    public void HostRandom()
    {
        Debug.Log("Tried to join a random room but failed. There must be no open games available.");
        
        var rRoom = Random.Range(100000, 1000000);

        roomName = "Random Room : " + rRoom;
        
        CreateOrJoinPublicRoom(roomName);
    }
    
    public override void OnCreatedRoom()
    {
        // base.OnCreatedRoom();
        Debug.Log("Created room");
        roomName = PhotonNetwork.CurrentRoom.Name;
        
        
        SceneManager.LoadScene(MyGameManager.Instance.onlineScene);
        
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        roomName = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Joined Room" + roomName);
        
        
        SceneManager.LoadScene(MyGameManager.Instance.onlineScene);
    }

    #endregion
    
    #region Private Room

    public void CreatePrivateRoom(string roomKey)
    {
        roomType = RoomType.Private;
        var roomOptions = new RoomOptions { IsVisible = false, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomKey, roomOptions);
    }
    
    public void JoinPrivateRoom(string roomKey)
    {
        roomType = RoomType.Private;
        PhotonNetwork.JoinRoom(roomKey);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    #endregion


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("Leave Room");
    }
    
}

public enum RoomType
{
    None,
    Private,
    Public
}