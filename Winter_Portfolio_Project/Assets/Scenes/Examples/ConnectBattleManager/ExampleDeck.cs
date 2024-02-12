using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WPP.DeckManagement.Example
{
    public class ExampleDeck : MonoBehaviour
    {
        [SerializeField] private DeckEditor _deckEditor;

        public void InitializeDeck()
        {
            _deckEditor.LoadDeck();
            for (int i = 0; i < _deckEditor.SelectedDeck.Cards.Count; i++)
            {
                _deckEditor.AddCard(CardDatabase.Cards.Values.ToList()[i]);
            }
        }
    }
}
