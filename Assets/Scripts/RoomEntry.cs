using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RoomEntry : MonoBehaviour
{
    public TMP_Text roomNameText;
    public TMP_Text roomNumberText;
    public TMP_Text roomPlayerAmountText;
    public Button joinRoomButton;
    public TMP_Text playerName;

    private string roomName;

    public void Start()
    {
        joinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(roomName);

         //   referenceName.text = GameObject.Find("RoomName").GetComponent<TextMeshProUGUI>().text;
           // referenceName.text= PhotonNetwork.JoinRoom(roomName).ToString();
        });
    }

    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName = name;

        roomNameText.text = name;
        roomPlayerAmountText.text = currentPlayers + " / " + maxPlayers;
    }
}
