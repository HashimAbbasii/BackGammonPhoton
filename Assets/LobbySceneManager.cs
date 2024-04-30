using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
    public static LobbySceneManager Instance;

    private void Awake()
    {
        Instance = this;

    }
}
