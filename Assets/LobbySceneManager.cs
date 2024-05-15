using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;

public class LobbySceneManager : MonoBehaviour
{
    public static LobbySceneManager Instance;
    private AudioManager _audioManager;

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

    public void Start()
    {
        _audioManager = GetComponent<AudioManager>();

        if (_audioManager.musicAudioSource.isPlaying)
            _audioManager.musicAudioSource.Stop();

        //_audioManager.musicAudioSource.clip = gameMusic;

        //_audioManager.musicAudioSource.loop = true;
        //_audioManager.musicAudioSource.Play();
    }
}
