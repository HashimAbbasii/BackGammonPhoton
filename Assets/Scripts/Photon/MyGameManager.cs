using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    #region Singleton

    public static MyGameManager Instance;
    public LanguageManager languageManager;
    public FullscreenWebGLManager fullScreenWebGLManager;

    //public FullscreenWebGLManager fullScreenWebGLManager;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    [Header("Scenes")]
    public SceneField offlineScene;
    public SceneField onlineScene;


    [Header("Other References")]
    public string playerTurn;
    public bool isMultiplayer;
    //public LanguageManager languageManager;
    public List<NetworkPlayer> players = new();

    //public int boardSize;
    //public int pawnRows;

    [ContextMenu("Test")]
    public void Test()
    {
        if (LobbySceneManager.Instance == null)
        {
            Debug.Log("Nothing");
        }
        else
        {
            Debug.Log("Found it");
        }

    }

    private void Start()
    {


    }
}
