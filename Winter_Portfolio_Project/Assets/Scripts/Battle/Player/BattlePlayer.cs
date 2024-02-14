using UnityEngine;
using WPP.DeckManagement;

namespace WPP.Battle
{
    public class BattlePlayer : MonoBehaviour
    {
        [SerializeField] private CrownSystem _crownSystem;
        [SerializeField] private TowerSystem _towerSystem;

        public CrownSystem CrownSystem => _crownSystem;
        public TowerSystem TowerSystem => _towerSystem;
    }
}

