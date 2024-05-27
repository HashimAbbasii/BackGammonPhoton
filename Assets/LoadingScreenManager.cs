using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{

    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public float fillSpeed = 5.5f;
    private float targetProgress = 0f;

    void Start()
    {
       // StartCoroutine(LoadMainMenu());
        progressBar.value = 0;
        progressText.text = "0%";
    }
    private void Update()
    {
      //  if (progressBar.value < targetProgress)
        {
            progressBar.value += fillSpeed * Time.deltaTime;
            progressText.text = Mathf.RoundToInt(progressBar.value) + "%";
            if (progressBar.value == 100)
            {
                LobbyManager.Instance.SwitchMenuView(true, false, false, false, false);

            }
        }
    }
    public IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(3f);
        LobbyManager.Instance.SwitchMenuView(true, false, false, false,false);
    }

    public void IncrementProgress(float newProgress)
    {
        targetProgress = progressBar.value + newProgress;
    }

}
