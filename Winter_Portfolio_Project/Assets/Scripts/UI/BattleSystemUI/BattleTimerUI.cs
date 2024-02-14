using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WPP.Battle.UI
{
    public class BattleTimerUI : MonoBehaviour
    {
        private BattleTimer _battleTimer;
        [SerializeField] private TextMeshProUGUI _timerText;

        private void OnEnable()
        {
            _battleTimer = BattleManager.Instance().BattleTimer;
            _battleTimer.OnTimerUpdate += SetTimerText;
        }
        private void OnDisable()
        {
            _battleTimer.OnTimerUpdate -= SetTimerText;
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
