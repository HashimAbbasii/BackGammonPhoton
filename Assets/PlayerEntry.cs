using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerEntry : MonoBehaviour
{
    public TMP_Text playerNameText;
    public GameObject meIndicator;

    //private string roomName;

    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        //roomName = name;

        playerNameText.text = name;
        
    }
}
