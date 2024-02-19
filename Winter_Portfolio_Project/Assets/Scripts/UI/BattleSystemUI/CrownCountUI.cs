using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WPP.Battle.UI
{
    public class CrownCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerText;
        [SerializeField] private TextMeshProUGUI _opponentText;

        private void OnEnable()
        {
            CrownSystem _player = BattleManager.Instance().Player.CrownSystem;
            CrownSystem _opponent = BattleManager.Instance().Opponent.CrownSystem;

            _player.OnCrownCountChange += SetPlayerCrownCount;
            _opponent.OnCrownCountChange += SetOpponentCrownCount;

            SetOpponentCrownCount(_opponent.CrownCount);
            SetPlayerCrownCount(_player.CrownCount);
        }

        private void SetOpponentCrownCount(int count)
        {
            _opponentText.text = count.ToString();
        }

        private void SetPlayerCrownCount(int count)
        {
            _playerText.text = count.ToString();
        }
    }
}
