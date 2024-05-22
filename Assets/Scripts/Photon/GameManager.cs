using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using BackgammonNet.Core;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private PhotonView photonView;
    //public  List<PhotonView> networkgameObjects = new List<PhotonView>();
    public NetworkPlayer myNetworkPlayer;
    public List<NetworkPlayer> networkPlayers = new();
    private List<PhotonView> photonViews = new List<PhotonView>();
    public static GameManager instance;

    public TextMeshProUGUI player0Name;
    public TextMeshProUGUI player1Name;

    public string WhoAmI;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, transform.position, transform.rotation);

        }
        //  CheckHierarchyForPhotonScripts(transform);
        CreateNetworkList();

        foreach (PhotonView view in photonViews)
        {
            Debug.Log("PhotonView attached to GameObject: " + view.gameObject.name);
            Debug.Log("Owner of PhotonView: " + view.Owner.NickName);
        }


        //if(MyGameManager.isNetworkGame == true)
        //{
        //    GameControllerNetwork.Instance.player0Name.text = MyGameManager.Instance.playerNames[0];
        //    GameControllerNetwork.Instance.player1Name.text = MyGameManager.Instance.playerNames[1];
        //}


        // if (PhotonNetwork.IsMasterClient)
        //  {
        // Set player names locally

       // if (MyGameManager.isNetworkGame == true)
       // {
            
            player0Name.text = MyGameManager.Instance.playerNames[0];
            player1Name.text = MyGameManager.Instance.playerNames[1];

            // Synchronize player names across the network
            photonView.RPC(nameof(UpdatePlayerNames), RpcTarget.AllBuffered,
                           MyGameManager.Instance.playerNames[0],
                           MyGameManager.Instance.playerNames[1]);
      //  }
        //  }


        //GameControllerNetwork.Instance.player0Name.variableText = MyGameManager.Instance.playerNames[0];
        //LanguageManager.OnVariableChanged();



        //GameControllerNetwork.Instance.player1Name.variableText = MyGameManager.Instance.playerNames[1];
        //LanguageManager.OnVariableChanged();

    }

    [PunRPC]
    void UpdatePlayerNames(string name0, string name1)
    {
        player0Name.text = name0;
        player1Name.text = name1;
    }




    // Update is called once per frame
    void Update()
    {
        WhoAmI = PhotonNetwork.NickName;
    }

    public void CreateNetworkList()
    {

        PhotonView[] photonViews = GameObject.FindObjectsOfType<PhotonView>();

        // Loop through each found PhotonView
        //foreach (PhotonView view in photonViews)
        //{
        //    // Add the PhotonView to the list
        //    networkgameObjects.Add(view);
        //}

        // Sort the list based on isMine property
        //networkgameObjects.Sort((x, y) =>
        //{
        //    // Sort by isMine property, with isMine=true objects first,
        //    // followed by isMine=false objects
        //    if (x.IsMine && !y.IsMine)
        //        return -1;
        //    else if (!x.IsMine && y.IsMine)
        //        return 1;
        //    else
        //        return 0;
        //});


        //PhotonNetwork.NickName = 



        // Now networkgameObjects list contains all game objects with PhotonView components
        // Objects with isMine = true will be at the top of the list


    }
    private void CheckHierarchyForPhotonScripts(Transform currentTransform)
    {
        // Check if the current GameObject has a PhotonView component attached
        PhotonView photonView = currentTransform.GetComponent<PhotonView>();

        foreach (PhotonView view in photonViews)
        {
            Debug.Log("Found GameObject with PhotonView component: " + view.gameObject.name);
            // Do whatever you need with the GameObject here
        }

        // Continue searching in the children of the current GameObject
        foreach (Transform child in currentTransform)
        {
            CheckHierarchyForPhotonScripts(child);
        }
    }



}

