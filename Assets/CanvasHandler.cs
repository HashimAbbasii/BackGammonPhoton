using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleLocalization.Scripts;

public class CanvasHandler : MonoBehaviour
{

    public GameObject diceRollButton;
    public GameObject diceResults;

    [Header("Bools")]
    public bool fullscreenToggle = false;
    public bool soundToggle = false;
    public bool musicToggle = false;
    private bool isToggle = true;

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

    public static CanvasHandler Instance;
    public FullscreenWebGLManager fullScreenWebGLManager;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (IsRunningOnAndroid())
            {
                fullScreenWebGLManager.EnterFullscreen();
            }
        }

        LocalizationManager.Read();


        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

#if UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif

    }

    public bool IsRunningOnAndroid()
    {
        return SystemInfo.operatingSystem.ToLower().Contains("android");

    }

    public bool IsRunningOniOS()
    {
        return SystemInfo.operatingSystem.ToLower().Contains("iphone") ||
               SystemInfo.operatingSystem.ToLower().Contains("ipad") ||
               SystemInfo.operatingSystem.ToLower().Contains("ios");
    }


    public void ToggleBoolSound()
    {
        Debug.Log("ToggleBoolSound");
        soundToggle = !soundToggle;

        AudioManager.Instance.ToggleVFXSound(!soundToggle);

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

        AudioManager.Instance.ToggleMusicSound(!musicToggle);

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
    public void OnPointerDown()
    {
        ToggleBoolean();
        ToggleBoolFullScreen(isToggle);
    }

    public void ToggleBoolean()
    {
        isToggle = !isToggle;
    }

    public void ToggleBoolFullScreen(bool istoggle)
    {
       // fullscreenToggle = !fullscreenToggle;

        if (istoggle)
        {

            MyGameManager.Instance.fullScreenWebGLManager.EnterFullscreen();

            //FullScreenR.gameObject.SetActive(true);
            //FullScreenL.gameObject.SetActive(false);

            fullScreenBtn.image.sprite = OnFullScreenToggle;
        }
        else
        {

            MyGameManager.Instance.fullScreenWebGLManager.ExitFullscreen();

            //FullScreenR.gameObject.SetActive(false);
            //FullScreenL.gameObject.SetActive(true);

            fullScreenBtn.image.sprite = OffFullScreenToggle;
        }
    }
   


    public void ButtonClicked()
    {
        AudioManager.Instance.ButtonClicked();
    }

}
