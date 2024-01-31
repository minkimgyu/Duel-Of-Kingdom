using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WPP.DeckManagement.Example
{
    public class DeckSystemUIExample : MonoBehaviour
    {
        [SerializeField] private DeckEditor _deckEditor;
        [SerializeField] private TextMeshProUGUI _text;

        private void Start()
        {
            _deckEditor.LoadDeck();
            _deckEditor.SelectDeck(0);
            UpdateText(0);
        }

        public void SelectDeck(int deckIndex)
        {
            _deckEditor.SelectDeck(deckIndex);
            UpdateText(deckIndex);
        }

        private void UpdateText(int deckIndex)
        {
            string deckInfo = "";
            deckInfo += "Selected Deck: " + deckIndex + "\n\n";
            for (int j = 0; j < 8; j++)
            {
                deckInfo += _deckEditor.SelectedDeck.GetCardId(j) + "\n";
            }
            _text.text = deckInfo;
        }
    }
}
