using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{

    public GameObject diceRollButton;
    public GameObject diceResults;

    [Header("Bools")]
    public bool fullscreenToggle = false;
    public bool soundToggle = false;
    public bool musicToggle = false;

    [Header("ButtonSprites")]
    public Sprite OnSoundToggle;
    public Sprite OffSoundToggle;
    public Sprite OnMusicToggle;
    public Sprite OffMusicToggle;
    public Sprite OnFullScreenToggle;
    public Sprite OffFullScreenToggle;

    [Header("TopMenuButtons")]
    public Button soundBtn;
    public Button musicBtn;
    public Button fullScreenBtn;



    public void ToggleBoolSound()
    {
        Debug.Log("ToggleBoolSound");
        soundToggle = !soundToggle;

        AudioManager.Instance.ToggleVFXSound(soundToggle);

        if (soundToggle)
        {
            Debug.Log("true");
            // VfxSoundToggleAnimator.Play("vfx sound Anim Reverse");
            soundBtn.image.sprite = OffSoundToggle;
        }
        else
        {
            Debug.Log("false");
            // VfxSoundToggleAnimator.Play("vfx sound Anim");
            soundBtn.image.sprite = OnSoundToggle;
        }
    }

    public void ToggleBoolMusic()
    {
        Debug.Log("ToggleBoolSound");
        musicToggle = !musicToggle;

        AudioManager.Instance.ToggleMusicSound(musicToggle);

        if (musicToggle)
        {
            Debug.Log("true");
            //musicSoundToggleAnimator.Play("Music Anim Reverse");
            musicBtn.image.sprite = OffMusicToggle;
        }
        else
        {
            Debug.Log("false");
            //musicSoundToggleAnimator.Play("Music Anim");
            musicBtn.image.sprite = OnMusicToggle;
        }

    }

    public void ToggleBoolFullScreen()
    {
        fullscreenToggle = !fullscreenToggle;

        if (fullscreenToggle)
        {
            //FullScreenToggleAnimator.Play("Full Screen Anim");
            fullScreenBtn.image.sprite = OnFullScreenToggle;
        }
        else
        {
            //FullScreenToggleAnimator.Play("Full Screen Anim Reverse");
            fullScreenBtn.image.sprite = OffFullScreenToggle;
        }
    }




}
