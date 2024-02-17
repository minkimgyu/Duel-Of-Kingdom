using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace WPP.Battle.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private float _gameOverPanelDelay = 1.5f;
        [Space]
        [SerializeField] private Color _unlockedCrownColor;
        [SerializeField] private Color _lockedCrownColor;
        [Space]
        [SerializeField] private GameObject _playerWinnerText;
        [SerializeField] private Image _playerCrown1;
        [SerializeField] private Image _playerCrown2;
        [SerializeField] private Image _playerCrown3;
        [Space]
        [SerializeField] private GameObject _opponentWinnerText;
        [SerializeField] private Image _opponentCrown1;
        [SerializeField] private Image _opponentCrown2;
        [SerializeField] private Image _opponentCrown3;

        private BattleManager _battleManager;
        private void OnEnable()
        {
            _battleManager = BattleManager.Instance();
            _battleManager.OnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            _battleManager.OnGameOver -= OnGameOver;
        }

        private async void OnGameOver(BattleResult result)
        {
            await Task.Delay(Mathf.RoundToInt(_gameOverPanelDelay * 1000));

            _gameOverPanel.SetActive(true);

            int playerCrown = _battleManager.Player.CrownSystem.CrownCount;
            int opponentCrown = _battleManager.Opponent.CrownSystem.CrownCount;

            _playerCrown1.color = playerCrown >= 1 ? _unlockedCrownColor : _lockedCrownColor;
            _playerCrown2.color = playerCrown >= 2 ? _unlockedCrownColor : _lockedCrownColor;
            _playerCrown3.color = playerCrown >= 3 ? _unlockedCrownColor : _lockedCrownColor;

            _opponentCrown1.color = opponentCrown >= 1 ? _unlockedCrownColor : _lockedCrownColor;
            _opponentCrown2.color = opponentCrown >= 2 ? _unlockedCrownColor : _lockedCrownColor;
            _opponentCrown3.color = opponentCrown >= 3 ? _unlockedCrownColor : _lockedCrownColor;

            if (result == BattleResult.Win)
            {
                _playerWinnerText.SetActive(true);
                _opponentWinnerText.SetActive(false);
            }
            else if (result == BattleResult.Lose)
            {
                _opponentWinnerText.SetActive(true);
                _playerWinnerText.SetActive(false);
            } else
            {
                _playerWinnerText.SetActive(false);
                _opponentWinnerText.SetActive(false);
            }
        }
    }
}
