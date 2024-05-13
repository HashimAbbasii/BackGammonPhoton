using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{

    #region Singleton

    public static AudioManager Instance;
    public AudioSource vfxAudioSource;
    public AudioSource musicAudioSource;
    public AudioMixer audioMixer;

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

        // if (audioSource)
        // {
        //     audioSource = GetComponent<AudioSource>();
        // }
    }

    #endregion

    [Header("Audio Clips")] 
    public AudioClip pawnPlacement;
 

    public AudioClip actionSuccess;
    public AudioClip actionFail;

    public AudioClip buttonClicked;
    
    public AudioClip gameWin;
    public AudioClip gameLose;
    
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    
    public void ButtonClicked()
    {
        vfxAudioSource.PlayOneShot(buttonClicked);
    }
    
    public void GameWon()
    {
        musicAudioSource.PlayOneShot(gameWin);
    }
    
    public void GameLost()
    {
        musicAudioSource.PlayOneShot(gameLose);
    }

    public void PlayMenuMusic()
    {
        if (musicAudioSource.isPlaying)
            musicAudioSource.Stop();
        musicAudioSource.clip = menuMusic;

        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
    
    public void PlayGameMusic()
    {
        if (musicAudioSource.isPlaying)
            musicAudioSource.Stop();
        musicAudioSource.clip = gameMusic;

        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
    
    public void BombDropped()
    {
        vfxAudioSource.PlayOneShot(pawnPlacement);
    }

    public void BombBlast()
    {
        vfxAudioSource.PlayOneShot(pawnPlacement);
    }

    public void BombWaterMiss()
    {
        vfxAudioSource.PlayOneShot(pawnPlacement);
    }

    public void PlaceShipInWater()
    {
        Invoke(nameof(PlaceShip), 0.3f);
    }

    private void PlaceShip()
    {
        if (vfxAudioSource.isPlaying)
            vfxAudioSource.Stop();
        vfxAudioSource.clip = pawnPlacement;
        //vfxAudioSource.Play();
    }

    public void ActionFailed()
    {
        vfxAudioSource.PlayOneShot(actionFail);
    }

    public void ActionSucceeded()
    {
        vfxAudioSource.PlayOneShot(actionSuccess);
    }

    public void ToggleMusicSound(bool toggle)
    {
        if (toggle)
        {
            audioMixer.SetFloat("Music", 0f);
        }
        else
        {
            audioMixer.SetFloat("Music", -80f);
        }
    }

    public void ToggleVFXSound(bool toggle)
    {
        if (toggle)
        {
            audioMixer.SetFloat("VFX", 0f);
        }
        else
        {
            audioMixer.SetFloat("VFX", -80f);
        }
    }
}
