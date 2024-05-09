using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class MyPhotonManager : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;
    public static MyPhotonManager Instance { get; set; }

    [Header("GAME OBJECT")]
    public GameObject roomListPrefab;
    public GameObject roomListParent;

    [Header("Text")]
    public TMP_InputField userNameText;

    public TMP_InputField roomNameText;
    public TMP_InputField maxPlayer;

    [Header("Panel")]

    public GameObject LobbyPanel;
    public GameObject PlayerNamePanel;
    public GameObject RoomCreatePanel;
    public GameObject ConnectingPanel;
    public GameObject Roomlist;
    private Dictionary<string, RoomInfo> roomListData;
    private Dictionary<string, GameObject> roomListGameObject;
    private Dictionary<int, GameObject> playerListGameObject;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;
    public GameObject playerListItemPrefab;
    public RectTransform playerListItemParent;
    public GameObject PlayButton;
    public static MyPhotonManager instance;

    //public DateTime startingTime;
    public bool multiPlayerMode = false;

    #region UnityMethod
    private void Awake()
    {
        instance = this;
    }
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }
    public void onCancelClick()
    {
        ActiveMyPanel(LobbyPanel.name);
    }
    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomListData.Clear();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "is connected to photon");
        ActiveMyPanel(LobbyPanel.name);

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();
        foreach (RoomInfo rooms in roomList)
        {
            Debug.Log("Room Name" + rooms.Name);
            if (!rooms.IsOpen || !rooms.IsVisible || rooms.RemovedFromList)

            {
                if (roomListData.ContainsKey(rooms.Name))
                {
                    roomListData.Remove(rooms.Name);
                }
            }

            else
            {
                if (roomListData.ContainsKey(rooms.Name))
                {
                    roomListData[rooms.Name] = rooms;
                }
                else
                {
                    roomListData.Add(rooms.Name, rooms);
                }
            }

            //  roomListData.Add(rooms.Name, rooms);
        }
        foreach (RoomInfo roomitem in roomListData.Values)
        {
            GameObject roomListItemObject = Instantiate(roomListPrefab, roomListParent.transform);
            roomListItemObject.transform.localPosition = Vector3.zero;
            //roomListItemObject.transform.SetParent();
            //roomListItemObject.transform.localPosition = Vector3.one;
            RectTransform rectTransform = roomListItemObject.GetComponent<RectTransform>();
            rectTransform.position = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            // Room name ...Room player...Button room Join ...//
            roomListItemObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = roomitem.Name;
            roomListItemObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roomitem.PlayerCount + "/" + roomitem.MaxPlayers;
            roomListItemObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => RoomJoinFromList(roomitem.Name));
            roomListGameObject.Add(roomitem.Name, roomListItemObject);
        }
    }

    #endregion

    #region UiMethod

    public void OnLoginClick()
    {
        string name = userNameText.text;
        if (!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            ActiveMyPanel(ConnectingPanel.name);

        }
        else
        {
            Debug.Log("Empty Name");
        }
    }
    public void RoomJoinFromList(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }
    #region _PhotonCallBack

    public void BackFromRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();

        }
        ActiveMyPanel(LobbyPanel.name);
    }

    public void BackFromPlayerList()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();

        }
        ActiveMyPanel(LobbyPanel.name);
    }



    public void OnClickRoomCreate()
    {

        string roomName = roomNameText.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            roomName = roomName + Random.Range(0, 1000);
        }

        RoomOptions roomOptions = new RoomOptions();
        //roomOptions.MaxPlayers = (byte)int.Parse(maxPlayer.text);
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName, roomOptions);

    }
    public override void OnCreatedRoom()
    {
        //base.OnCreatedRoom();
        Debug.Log(PhotonNetwork.CurrentRoom.Name + "is created !");
    }

    public override void OnJoinedRoom()
    {
        //base.OnCreatedRoom();
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "Room Joined");
        ActiveMyPanel(InsideRoomPanel.name);
        if (playerListGameObject == null)
        {
            playerListGameObject = new Dictionary<int, GameObject>();
        }

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(playerListItemPrefab, playerListItemParent.transform);
            playerListItem.transform.localPosition = Vector3.zero;
            //playerListItem.transform.SetParent(playerListItemParent.transform);
            //playerListItem.transform.localScale = Vector3.one;
            RectTransform rectTransform = playerListItem.GetComponent<RectTransform>();
            rectTransform.position = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = p.NickName;
            if (p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(false);
            }
            playerListGameObject.Add(p.ActorNumber, playerListItem);
        }


    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(playerListItemPrefab, playerListItemParent.transform);
        playerListItem.transform.localPosition = Vector3.zero;
        //playerListItem.transform.SetParent(playerListItemParent.transform);
        //playerListItem.transform.localScale = Vector3.one;
        RectTransform rectTransform = playerListItem.GetComponent<RectTransform>();
        rectTransform.position = Vector2.zero;


        playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(false);
        }
        playerListGameObject.Add(newPlayer.ActorNumber, playerListItem);
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PlayButton.SetActive(true); // Enable the Play button
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGameObject[otherPlayer.ActorNumber]);
        playerListGameObject.Remove(otherPlayer.ActorNumber);
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PlayButton.SetActive(true); // Enable the Play button
        }
        else
        {
            PlayButton.SetActive(false); // Disable the Play button
        }
    }
    public override void OnLeftRoom()
    {
        ActiveMyPanel(LobbyPanel.name);
        foreach (GameObject obj in playerListGameObject.Values)
        {
            Destroy(obj);
        }
    }
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        roomListData = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
        //startingTime = DateTime.Now;
        StartCoroutine(Test());
    }


    IEnumerator Test()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, 10));
            //Debug.Log(DateTime.Now.Ticks - startingTime.Ticks);
        }

    }

    #region _PublicMethod

    public void ActiveMyPanel(GameObject panel)
    {
        LobbyPanel.SetActive(panel.Equals(LobbyPanel));
    }

    public void ActiveMyPanel(string panelName)
    {
        LobbyPanel.SetActive(panelName.Equals(LobbyPanel.name));
        PlayerNamePanel.SetActive(panelName.Equals(PlayerNamePanel.name));
        RoomCreatePanel.SetActive(panelName.Equals(RoomCreatePanel.name));
        ConnectingPanel.SetActive(panelName.Equals(ConnectingPanel.name));
        Roomlist.SetActive(panelName.Equals(Roomlist.name));
        InsideRoomPanel.SetActive(panelName.Equals(InsideRoomPanel.name));
    }
    public void ClearRoomList()
    {
        if (roomListGameObject.Count > 0)
        {
            foreach (var v in roomListGameObject.Values)
            {
                Destroy(v);
            }
            roomListGameObject.Clear();
        }
    }
    public void RoomListBtnClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActiveMyPanel(Roomlist.name);
    }
    
    public void OnClickPlayButton()
    {
        _photonView.RPC(nameof(MultiplayerModeRPC), RpcTarget.AllBuffered);
        if (PhotonNetwork.IsMasterClient)
        {
            if (SceneManager.sceneCount > 2)
            {
                SceneManager.UnloadSceneAsync(2);
            }

            // Load the scene additively
            PhotonNetwork.LoadLevel(2);
        }
    }
    #endregion

    [PunRPC]
    public void MultiplayerModeRPC()
    {
        multiPlayerMode = true;
    }
}