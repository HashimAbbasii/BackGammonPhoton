using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public PhotonView photonView;

    public string playerName;
    
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        GameNetworkSetup.Instance.players.Add(this);

        if (photonView.Controller.IsMasterClient && photonView.IsMine)
        {
            Debug.Log("I am master client");
        }

        if (GameNetworkSetup.Instance.players.Count > 1)
        {
            if (photonView.Controller.IsMasterClient)
            {
                int[] viewIDs = new int[GameNetworkSetup.Instance.players.Count];
                for (var i = 0; i < GameNetworkSetup.Instance.players.Count; i++)
                {
                    var player = GameNetworkSetup.Instance.players[i];
                    viewIDs[i] = player.photonView.ViewID;
                }
                                
                photonView.RPC(nameof(OrderPlayers), RpcTarget.AllBuffered, viewIDs);
            }
        }
    }

    [PunRPC]
    public void OrderPlayers(int[] viewIDs)
    {
        for (var index = 0; index < viewIDs.Length; index++)
        {
            var vid = viewIDs[index];
            MyGameManager.Instance.players.Add(PhotonView.Find(vid).GetComponent<NetworkPlayer>());
        }
        MyGameManager.Instance.players.Reverse();

        
        for (var i = 0; i < MyGameManager.Instance.players.Count; i++)
        {
            var p = MyGameManager.Instance.players[i];
            p.playerName = "Player " + (i + 1);
        }
        
        //MyGameManager.Instance.boardSize = boardSize;
        //MyGameManager.Instance.pawnRows = pawnRows;
        
        //GameNetworkSetup.Instance.mainBoardHandler.StartGame();
    }
}
