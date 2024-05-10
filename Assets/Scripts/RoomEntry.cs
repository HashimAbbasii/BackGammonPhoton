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
        });
    }

    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName = name;

        roomNameText.text = name;
        roomPlayerAmountText.text = currentPlayers + " / " + maxPlayers;
    }
}
