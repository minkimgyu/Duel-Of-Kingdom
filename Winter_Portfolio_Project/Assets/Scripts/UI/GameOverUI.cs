using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Battle.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [Space]
        [SerializeField] private GameObject _playerWinnerText;
        [SerializeField] private GameObject _playerCrown1;
        [SerializeField] private GameObject _playerCrown2;
        [SerializeField] private GameObject _playerCrown3;
        [Space]
        [SerializeField] private GameObject _opponentWinnerText;
        [SerializeField] private GameObject _opponentCrown1;
        [SerializeField] private GameObject _opponentCrown2;
        [SerializeField] private GameObject _opponentCrown3;

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

        private void OnGameOver(BattleResult result)
        {
            _gameOverPanel.SetActive(true);

            int playerCrown = _battleManager.Player.CrownSystem.CrownCount;
            int opponentCrown = _battleManager.Opponent.CrownSystem.CrownCount;
            
            _playerCrown1.SetActive(playerCrown >= 1);
            _playerCrown2.SetActive(playerCrown >= 2);
            _playerCrown3.SetActive(playerCrown >= 3);

            _opponentCrown1.SetActive(opponentCrown >= 1);
            _opponentCrown2.SetActive(opponentCrown >= 2);
            _opponentCrown3.SetActive(opponentCrown >= 3);

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
