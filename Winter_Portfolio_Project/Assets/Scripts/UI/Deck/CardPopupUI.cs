using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Deck.UI
{
    public class CardPopupUI : MonoBehaviour
    {
        [SerializeField] private GameObject _addButton;
        [SerializeField] private GameObject _removeButton;

        private DeckUIController _controller;
        private int _gridIndex;

        public void Initialize(DeckUIController controller, int gridIndex, bool isDeckPopup)
        {
            _controller = controller;
            _gridIndex = gridIndex;

            if (isDeckPopup)
            {
                _addButton.SetActive(false);
                _removeButton.SetActive(true);
            }
            else
            {
                _addButton.SetActive(true);
                _removeButton.SetActive(false);
            }
        }

        public void AddToDeck()
        {
            _controller.AddCardToDeck(_gridIndex);
        }
        public void RemoveFromDeck()
        {
            _controller.RemoveCardFromDeck(_gridIndex);
        }
    }
}
