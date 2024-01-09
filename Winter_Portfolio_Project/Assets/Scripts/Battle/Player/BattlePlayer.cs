using UnityEngine;

namespace WPP.Battle
{
    public class BattlePlayer : MonoBehaviour
    {
        [Header("Tower System")]
        [SerializeField] private TowerSystem _towerSystem;
        [Header("Crown System")]
        [SerializeField] private CrownSystem _crownSystem;
        [Header("Elixir System")]
        [SerializeField] private ElixirSystem _elixirSystem;
    }
}

