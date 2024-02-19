using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WPP.Battle.UI
{
    public class BattleTimerUI : MonoBehaviour
    {
        private BattleTimer _battleTimer;
        private ElixirSystem _elixirSystem;
        private BattleManager _battleManager;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private GameObject _overtimeText;
        [SerializeField] private TextMeshProUGUI _elixirRegenTimeText;

        private void OnEnable()
        {
            _overtimeText.SetActive(false);
            _elixirRegenTimeText.gameObject.SetActive(false);

            _battleManager = BattleManager.Instance();
            _battleTimer = _battleManager.BattleTimer;
            _elixirSystem = _battleManager.ElixirSystem;

            _battleManager.OnStatusChange += OnBattleStatusChange;
            _battleTimer.OnTimerUpdate += SetTimerText;
            _elixirSystem.OnElixirRegenTimeChange += ShowElixirRegenTime;
        }

        private void OnDisable()
        {
            _battleManager.OnStatusChange -= OnBattleStatusChange;
            _battleTimer.OnTimerUpdate -= SetTimerText;
            _elixirSystem.OnElixirRegenTimeChange -= ShowElixirRegenTime;
        }

        private void SetTimerText(float elapsedTime, float timerLength)
        {
            var timeLeft = timerLength - elapsedTime;
            var sec = Mathf.FloorToInt(timeLeft % 60);
            var min = Mathf.FloorToInt(timeLeft / 60);

            _timerText.text = $"{min:00}:{sec:00}";
        }

        private void OnBattleStatusChange(BattleManager.Status status)
        {
            if (status == BattleManager.Status.Overtime)
            {
                _overtimeText.SetActive(true);
            }
        }

        private void ShowElixirRegenTime(float regenTime)
        {
            if(regenTime == _battleManager.BattleRegenRate)
            {
                _elixirRegenTimeText.gameObject.SetActive(false);
            }
            else if(regenTime == _battleManager.OvertimeRegenRate1)
            {
                _elixirRegenTimeText.gameObject.SetActive(true);
                _elixirRegenTimeText.text = "x2";
            } 
            else if(regenTime == _battleManager.OvertimeRegenRate2)
            {
                _elixirRegenTimeText.gameObject.SetActive(true);
                _elixirRegenTimeText.text = "x3";
            }
        }
    }
}
