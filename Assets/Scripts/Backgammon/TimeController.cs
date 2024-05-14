using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace BackgammonNet.Core
{
    public class TimeController : MonoBehaviour
    {
        public static event Action<bool> OnTimeLimitEnd = delegate { };
                
        public float[] timeLapse = new float[2];
        private float timeRange = 600;              // each player's playing time in seconds
        private float timeInterval = 1f;            // how often the clock is updated
        private float timeElapsed;                  // elapsed time since the last update
        [SerializeField] private Text timeDisplay;
        [SerializeField] private TMP_Text timeDisplayPanel;
        [SerializeField] private TMP_Text timeDisplayPanel2;
        [SerializeField] private TMP_Text timeDisplayPanel3;


        public static TimeController Instance { get; set; }

        private void Awake()
        {
            Instance = this;
            timeElapsed = timeInterval;

            UpdateDisplay(0.1f);
        }

        private void Update()
        {
            if (!GameController.GameOver && Board.Instance.acceptance >= 2)
            {
                //GetComponent<GameController>().canvasHandler.diceRollButton.SetActive(true);
                //GetComponent<GameController>().canvasHandler.diceResults.SetActive(true);

                float time;

                timeLapse[GameController.turn] += Time.deltaTime;
                time = timeLapse[GameController.turn];

                if ((timeElapsed += Time.deltaTime) >= timeInterval)
                {
                    timeElapsed = 0;
                    UpdateDisplay(time);
                }
            }
        }

        private void UpdateDisplay(float time)
        {
            if (timeDisplay.text == "00:00")             // player has timed out
            {
                OnTimeLimitEnd(GameController.turn != 0);
                Destroy(this);
                return;
            }

         

            time = timeRange + 1 - time;

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            timeDisplay.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            timeDisplayPanel.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            timeDisplayPanel2.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            timeDisplayPanel3.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        }
    }
}