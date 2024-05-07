using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using System;

public class NetworkPlayer : MonoBehaviour
{
    public PhotonView photonView;

    public string playerName;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player Created");

        GameManager.instance.networkPlayers.Add(this);

        GameManager.instance.networkPlayers = GameManager.instance.networkPlayers.OrderBy(ch=>ch.photonView.ViewID).ToList();

        foreach (var view in GameManager.instance.networkPlayers.Where(vw => vw.photonView.IsMine))
        {
            PhotonNetwork.NickName = GameManager.instance.networkPlayers.IndexOf(view).ToString();
            GameManager.instance.myNetworkPlayer = view;
        }

    }

    private void Update()
    {
        
    }
}
