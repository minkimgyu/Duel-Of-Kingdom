using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Timers;

namespace WPP
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timeTMP;
        private static float timeLeft;
        void Start()
        {
            timeLeft = 180.0f;
        }

        public void Update()
        {
            timeLeft -= Time.deltaTime;
            OnTimerTick();
        }

        private void OnTimerTick()
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            _timeTMP.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public static void HandleDoubleElixirTime()
        {
            Debug.Log("DoubleElixirTime");
        }
        public static void HandleSuddenDeathTime()
        {
            timeLeft = 120.0f;
            Debug.Log("SuddenDeathTime");
        }
    }

}