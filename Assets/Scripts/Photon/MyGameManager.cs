using BackgammonNet.Lobby;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyGameManager : MonoBehaviour
{
    #region Singleton
   
    public static MyGameManager Instance;
    public static bool AiMode = false;
    public static bool HumanMode = false;
    public static bool isNetworkGame = false;
    public LanguageManager languageManager;
    public FullscreenWebGLManager fullScreenWebGLManager;
    public Difficulty botDifficulty = Difficulty.None;
    //public FullscreenWebGLManager fullScreenWebGLManager;
    [SerializeField] public Sprite whitePawn;
    [SerializeField] public Sprite blackPawn;
    public Sprite StoreWhite;
    public Sprite StoreBlack;
    public int randomSelect;
    public Sprite[] randomImage;
    public static bool AiModePawn = false;
    public bool AiModeTest;
    public static int houseColorDetermince;

    //  public void  Start()
    //{

    //}



    //public void gameOverOnHumanPanel()
    //{

    //}
    private void Update()
    {
        //AiModeTest = AiModePawn;
        AiModeTest = AiMode;
    }

    public void SelectPawn(int pawnSelect)
    {
        switch (pawnSelect)
        {
            case 0:
                AiModePawn = true;
                StoreWhite = whitePawn;
                StoreBlack = blackPawn;
                houseColorDetermince = 0;
                break;

            case 1:
                {
                    randomSelect = UnityEngine.Random.Range(0, randomImage.Length);

                    for (int i = 0; i < randomImage.Length; i++)
                    {
                        switch (randomSelect)
                        {
                            case 0:
                                AiModePawn = true;
                                StoreWhite = whitePawn;
                                StoreBlack = blackPawn;
                                break;

                            case 1:
                                AiModePawn = true;
                                StoreWhite = blackPawn;
                                StoreBlack = whitePawn;
                                break;
                        }
                    }
                    houseColorDetermince = 1;
                    break;
                    //StoreWhite = (UnityEngine.Random.value > 0.5f) ? whitePawn : blackPawn;
                }

            case 2:
                AiModePawn = true;
                StoreWhite = blackPawn;
                StoreBlack = whitePawn;
                houseColorDetermince = 2;
                break;
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