using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WPP.Battle.UI
{
    public class BattleTimerUI : MonoBehaviour
    {
        private BattleTimer _battleTimer;
        private BattleManager _battleManager;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private GameObject _overtimeText;

        private void OnEnable()
        {
            _overtimeText.SetActive(false);

            _battleTimer = BattleManager.Instance().BattleTimer;
            _battleTimer.OnTimerUpdate += SetTimerText;
            _battleManager = BattleManager.Instance();
            _battleManager.OnStatusChange += OnBattleStatusChange;
        }
        private void OnDisable()
        {
            _battleTimer.OnTimerUpdate -= SetTimerText;
            _battleManager.OnStatusChange -= OnBattleStatusChange;
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
    }
}
