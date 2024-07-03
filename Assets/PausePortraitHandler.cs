using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePortraitHandler : MonoBehaviour
{
    [SerializeField] public GameObject portraitTopBar;
    [SerializeField] public GameObject landscapeTopBar;

    private void Update()
    {
        StartCoroutine(RotateCameraAndCanvas());
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


    public IEnumerator RotateCameraAndCanvas()
    {
        float ratio = (Screen.width * 1f / Screen.height);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (IsRunningOnAndroid() || IsRunningOniOS())
            {
                if (ratio < 1)  //_ratio  Portrait Android/iOS
                {
                    portraitTopBar.gameObject.SetActive(true);
                    landscapeTopBar.gameObject.SetActive(false);

                }

                else if (ratio >= 2) //_ratio  LAndScape Android/iOS
                {
                    portraitTopBar.gameObject.SetActive(false);
                    landscapeTopBar.gameObject.SetActive(true);
                }




            }

        }

        yield return null;

    }
}
