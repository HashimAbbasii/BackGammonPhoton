using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using System;
using BackgammonNet.Core;

public class NetworkPlayer : MonoBehaviour
{
    public PhotonView photonView;

    public string playerName;

    private int _score;
    private int _moves;
    private int _kills;
    private int _shelter;
    private int _time;

    public int Score
    {
        get => _score;
        set
        {
            _score = 2 * _moves + 20 * _shelter + 10 * _kills - 2 * _time;



            GameControllerNetwork.Instance.scoreTextPausePanel.variableText = GameManager.instance.myNetworkPlayer.Score.ToString();
            LanguageManager.OnVariableChanged();

            GameControllerNetwork.Instance.scoreTextgameOverPausePanel.variableText = GameManager.instance.myNetworkPlayer.Score.ToString();
            LanguageManager.OnVariableChanged();

            GameControllerNetwork.Instance.scoreTextyouWinPausePanel.variableText = GameManager.instance.myNetworkPlayer.Score.ToString(); ;
            LanguageManager.OnVariableChanged();

            GameControllerNetwork.Instance.scoreTextdefaultYouWinPausePanel.variableText = GameManager.instance.myNetworkPlayer.Score.ToString();
            LanguageManager.OnVariableChanged();


        }
    }

    public int Moves
    {
        get => _moves;
        set
        {
            _moves = value;
            Score++;
        }
    }

    public int Kills
    {
        get => _kills;
        set
        {
            _kills = value;
            Score++;
        }
    }
    public int Shelter
    {
        get => _shelter;
        set
        {
            _shelter = value;
            Score++;
        }
    }
    public int TotalTime
    {
        get => _time;
        set
        {
            _time = value;
            Score++;
        }
    }

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
