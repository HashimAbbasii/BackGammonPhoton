using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;

public class LobbySceneManager : MonoBehaviour
{
    public static LobbySceneManager Instance;

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

    public GameObject connectionPanel;
    public GameObject lobbyCanvas;

}
