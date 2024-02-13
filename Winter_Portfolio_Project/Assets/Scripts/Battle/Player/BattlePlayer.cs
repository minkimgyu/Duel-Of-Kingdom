using UnityEngine;
using WPP.DeckManagement;

namespace WPP.Battle
{
    public class BattlePlayer : MonoBehaviour
    {
        [Header("Tower System")]
        [SerializeField] private TowerSystem _towerSystem;
        [Header("Crown System")]
        [SerializeField] private CrownSystem _crownSystem;
        [Header("Deck System")]
        [SerializeField] private DeckSystem _deckSystem;
        [Header("Elixir System")]
        [SerializeField] private ElixirSystem _elixirSystem;

        public TowerSystem Tower => _towerSystem;
        public CrownSystem Crown => _crownSystem;
        public DeckSystem Deck => _deckSystem;
        public ElixirSystem Elixir => _elixirSystem;

        public void Init()
        {
            _deckSystem.Init(DeckManager.CurrentDeck);
        }
    }
}

