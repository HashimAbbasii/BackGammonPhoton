using BackgammonNet.Lobby;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    #region Singleton
   
    public static MyGameManager Instance;
    public static bool AiMode = false;
    public static bool isNetworkGame = false;
    public LanguageManager languageManager;
    public FullscreenWebGLManager fullScreenWebGLManager;
    public Difficulty botDifficulty = Difficulty.None;
    //public FullscreenWebGLManager fullScreenWebGLManager;
    [SerializeField] public Sprite whitePawn;
    [SerializeField] public Sprite blackPawn;
    public Sprite StoreWhite;
    public Sprite StoreBlack;

    //  public void  Start()
    //{

    //}

    public void SelectPawn(int pawnSelect)
    {
        if (pawnSelect == 0)
        {
            StoreWhite = whitePawn;
            StoreBlack = blackPawn;
        }
        else if (pawnSelect == 1)
        {
            StoreWhite = (UnityEngine.Random.value > 0.5f) ? whitePawn : blackPawn;
        }
        else
        {
            StoreWhite = blackPawn;
            StoreBlack = blackPawn;
        }
    }


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
    public List<string> playerNames = new();

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
    private GameObject menu;
    public void SceneShiftPanel()
    {
        StartCoroutine(MenuMangerPanel());
    }
    IEnumerator MenuMangerPanel()
    {
        yield return null;
        LobbyManager.Instance.SwitchMenuView(true, false, false, false, false);
    }


}


public enum Difficulty
{
    None,
    Beginner,
    Intermediate,
    GrandMaster
}