using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;

public class LoadingScreenManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadMainMenu());
    }

    public IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(3f);
        LobbyManager.Instance.SwitchMenuView(true, false, false, false,false);
    }

}
