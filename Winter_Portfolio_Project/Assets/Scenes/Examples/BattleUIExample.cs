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
        [System.Serializable]
        private class TowerUIGroup
        {
            public Tower tower;
            public TextMeshProUGUI hpText;
            public Button damageButton;

            public Action<int, int> OnDamaged;
            public Action<Tower> OnDestroyed;
        }

        [Header("Battle Manager")]
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Button _startButton;

        [Header("Timer")]
        [SerializeField] private BattleTimer _battleTimer;
        [SerializeField] private TextMeshProUGUI _timerText;
        
        [Header("Player Tower")]
        [SerializeField] private TowerSystem _playerTowerSystem;
        [SerializeField] private TowerUIGroup _playerCT1;
        [SerializeField] private TowerUIGroup _playerCT2;
        [SerializeField] private TowerUIGroup _playerKT;
        [Header("Opponent Tower")]
        [SerializeField] private TowerSystem _opponentTowerSystem;
        [SerializeField] private TowerUIGroup _opponentCT1;
        [SerializeField] private TowerUIGroup _opponentCT2;
        [SerializeField] private TowerUIGroup _opponentKT;


        private void OnEnable()
        {
            _battleManager.OnStatusChange += SetStatusText;
            _battleTimer.OnTimerUpdate += SetTimerText;

            _startButton.onClick.AddListener(() => _battleManager.StartBattle());

            // Player Tower
            _playerCT1.damageButton.onClick.AddListener(() => _playerCT1.tower.Damage(1));
            _playerCT2.damageButton.onClick.AddListener(() => _playerCT2.tower.Damage(1));
            _playerKT.damageButton.onClick.AddListener(() => _playerKT.tower.Damage(1));

            SubscribeTowerEvent(_playerCT1);
            SubscribeTowerEvent(_playerCT2);
            SubscribeTowerEvent(_playerKT);

            // Opponent Tower
            _opponentCT1.damageButton.onClick.AddListener(() => _opponentCT1.tower.Damage(1));
            _opponentCT2.damageButton.onClick.AddListener(() => _opponentCT2.tower.Damage(1));
            _opponentKT.damageButton.onClick.AddListener(() => _opponentKT.tower.Damage(1));

            SubscribeTowerEvent(_opponentCT1);
            SubscribeTowerEvent(_opponentCT2);
            SubscribeTowerEvent(_opponentKT);
        }

        private void OnDisable()
        {
            _battleManager.OnStatusChange -= SetStatusText;
            _battleTimer.OnTimerUpdate -= SetTimerText;

            _startButton.onClick.RemoveAllListeners();

            _playerCT1.damageButton.onClick.RemoveAllListeners();
            _playerCT2.damageButton.onClick.RemoveAllListeners();
            _playerKT.damageButton.onClick.RemoveAllListeners();

            UnsubscribeTowerEvent(_playerCT1);
            UnsubscribeTowerEvent(_playerCT2);
            UnsubscribeTowerEvent(_playerKT);

            _opponentCT1.damageButton.onClick.RemoveAllListeners();
            _opponentCT2.damageButton.onClick.RemoveAllListeners();
            _opponentKT.damageButton.onClick.RemoveAllListeners();

            UnsubscribeTowerEvent(_opponentCT1);
            UnsubscribeTowerEvent(_opponentCT2);
            UnsubscribeTowerEvent(_opponentKT);

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

        private void SubscribeTowerEvent(TowerUIGroup towerUIGroup)
        {
            towerUIGroup.OnDamaged = SetTowerHPText;
            void SetTowerHPText(int curHP, int maxHP)
            {
                towerUIGroup.hpText.text = $"{curHP}/{maxHP}";
            }
            towerUIGroup.OnDestroyed += (tower) =>
            {
                Debug.Log($"{tower.name} is destroyed!");
                towerUIGroup.hpText.text = "Destroyed";
            };

            towerUIGroup.tower.OnDamaged += towerUIGroup.OnDamaged;
            towerUIGroup.tower.OnDestroyed += towerUIGroup.OnDestroyed;
        }
        private void UnsubscribeTowerEvent(TowerUIGroup towerUIGroup)
        {
            towerUIGroup.tower.OnDamaged -= towerUIGroup.OnDamaged;
            towerUIGroup.tower.OnDestroyed -= towerUIGroup.OnDestroyed;
        }
    }
}
