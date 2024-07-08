using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackgammonNet.Lobby;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject BottomMenu;


    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public float fillSpeed = 5.5f;
    private float targetProgress = 0f;
    private bool isStuck = false;
    void Start()
    {
       // StartCoroutine(LoadMainMenu());
        progressBar.value = 0;
        progressText.text = "0%";
        StartCoroutine(UpdateProgress());
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
               // BottomMenu.gameObject.SetActive(true);

            }
        }
    }


    IEnumerator UpdateProgress()
    {
        while (progressBar.value < 100)
        {
            if (!isStuck)
            {
                progressBar.value += fillSpeed * Time.deltaTime ;
                progressText.text = Mathf.RoundToInt(progressBar.value) + "%";
                if (Mathf.Approximately(progressBar.value, 100f))
                {
                    LobbyManager.Instance.SwitchMenuView(true, false, false, false, false);
                    yield break;
                }
            }

            if (Random.value < 2f && !isStuck) // 1% chance to get "stuck"
            {
                isStuck = true;
                yield return new WaitForSeconds(Random.Range(4f, 6f)); // Random delay between 0.5 and 2 seconds
                isStuck = false;
            }
            yield return null;
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
