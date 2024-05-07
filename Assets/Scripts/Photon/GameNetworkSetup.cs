using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameNetworkSetup : MonoBehaviourPunCallbacks
{
    #region Singleton

    public static GameNetworkSetup Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
    
    public List<NetworkPlayer> players;
    //public MainBoardHandler mainBoardHandler;

    public void Start()
    {
        // PhotonNetwork.Instantiate("NPlayer", Vector3.zero, Quaternion.identity);
    }

    private void Update()
    {
       // if (!photonView.IsMine) return;

        //Debug.Log(PhotonNetwork.PlayerList.Length);
        
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    Debug.Log("Player Enter S2");
    //    base.OnPlayerEnteredRoom(newPlayer);
    //    RoomJoined();
    //}

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("Room Joined S2");
    //    base.OnJoinedRoom();
    //    RoomJoined();
    //}




    public void RoomJoined()
    {
        Debug.Log("Room Joined, Create Player");
        
        var player = PhotonNetwork.Instantiate("NetworkPlayer", Vector3.zero, Quaternion.identity);
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        
        //mainBoardHandler.EndGame();
    }
}
