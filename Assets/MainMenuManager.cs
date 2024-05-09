using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public HorizontalLayoutGroup topMenu;
    public List<GameObject> buttons;

    public bool menuToggle;

    public void MenuButtonToggle()
    {
        if (menuToggle)
        {
            menuToggle = false;

            StopAllCoroutines();
            StartCoroutine(AnimateTopMenu(false));
        }
        else
        {
            menuToggle = true;
            foreach (var button in buttons)
            {
                button.SetActive(true);
            }
            StopAllCoroutines();
            StartCoroutine(AnimateTopMenu(true));
        }
    }

    private IEnumerator AnimateTopMenu(bool toggle)
    {
        float elapsedTime = 0;
        float percentageComplete = 0;

        if (toggle)
        {
            while (topMenu.spacing < 10f)
            {
                elapsedTime += Time.deltaTime;
                percentageComplete = elapsedTime / 1.8f;

                topMenu.spacing = Mathf.Lerp(topMenu.spacing, 10f, percentageComplete);

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (topMenu.spacing > -580f)
            {
                elapsedTime += Time.deltaTime;
                percentageComplete = elapsedTime / 1.8f;

                topMenu.spacing = Mathf.Lerp(topMenu.spacing, -580f, percentageComplete);

                yield return new WaitForFixedUpdate();
            }

            foreach (var button in buttons)
            {
                button.SetActive(false);
            }
        }
    }
}
