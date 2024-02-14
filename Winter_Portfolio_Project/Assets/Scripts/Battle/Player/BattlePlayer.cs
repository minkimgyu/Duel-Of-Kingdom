using UnityEngine;

namespace WPP.Battle
{
    public class BattlePlayer : MonoBehaviour
    {
        [SerializeField] private BattlePlayer _enemy;
        [Space]
        [SerializeField] private CrownSystem _crownSystem;
        [SerializeField] private TowerSystem _towerSystem;

        public void Init()
        {
            _enemy._towerSystem.OnKingTowerDestroyed += _crownSystem.AddCrown;
            _enemy._towerSystem.OnLeftPrincessTowerDestroyed += _crownSystem.AddCrown;
            _enemy._towerSystem.OnRightPrincessTowerDestroyed += _crownSystem.AddCrown;
        }

        public CrownSystem CrownSystem => _crownSystem;
        public TowerSystem TowerSystem => _towerSystem;
    }
}

