using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WPP.DeckManagement.UI
{
    public class CardPopupUI : MonoBehaviour
    {
        [SerializeField] private GameObject _addButton;
        [SerializeField] private GameObject _removeButton;
        [Space]
        [SerializeField] private TextMeshProUGUI _cardLv;
        [SerializeField] private Button _levelUp;
        [SerializeField] private Button _levelDown;

        private DeckEditorUIController _controller;
        private int _gridIndex;

        public void Initialize(DeckEditorUIController controller, int gridIndex, bool isDeckPopup)
        {
            _controller = controller;
            _gridIndex = gridIndex;

            if (isDeckPopup)
            {
                _addButton.SetActive(false);
                _removeButton.SetActive(true);

                _levelUp.interactable = true;
                _levelDown.interactable = true;
            }
            else
            {
                _addButton.SetActive(true);
                _removeButton.SetActive(false);

                _cardLv.text = "-";
                _levelUp.interactable = false;
                _levelDown.interactable = false;
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

        public void SetCardLevel(int level)
        {
            _cardLv.text = level.ToString();
        }

        public void LevelUp()
        {
            _controller.LevelUpCard(_gridIndex);
        }
        public void LevelDown()
        {
            _controller.LevelDownCard(_gridIndex);
        }
    }
}
