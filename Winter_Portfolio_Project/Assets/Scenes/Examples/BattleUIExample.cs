using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace WPP.Battle.Example
{
    public class BattleUIExample : MonoBehaviour
    {
        [Header("Battle Manager")]
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Button _startButton;
        [Header("Timer")]
        [SerializeField] private BattleTimer _battleTimer;
        [SerializeField] private TextMeshProUGUI _timerText;

        private void OnEnable()
        {
            _battleManager.OnStatusChange += SetStatusText;
            _battleTimer.OnTimerUpdate += SetTimerText;

            _startButton.onClick.AddListener(() => _battleManager.StartBattle());
        }
        private void OnDisable()
        {
            _battleManager.OnStatusChange -= SetStatusText;
            _battleTimer.OnTimerUpdate -= SetTimerText;
        }

        private void SetStatusText(BattleManager.Status status)
        {
            _statusText.text = "Battle status : " + status.ToString();
        }

        private void SetTimerText(float elapsedTime, float timerLength)
        {
            var timeLeft = timerLength - elapsedTime;
            var sec = Mathf.FloorToInt(timeLeft % 60);
            var min = Mathf.FloorToInt(timeLeft / 60);

            _timerText.text = $"{min:00}:{sec:00}";
        }
    }
}
