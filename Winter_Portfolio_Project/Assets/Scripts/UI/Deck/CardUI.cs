using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WPP.Deck.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _cardName;
        [SerializeField] private Button _button;

        [SerializeField] private RectTransform _levelBar;

        private DeckUIController _controller;
        private GameObject _popup;
        private int _gridIndex;


        public void Initialize(DeckUIController controller, GameObject popup, int gridIndex)
        {
            _controller = controller;
            _popup = popup;
            _gridIndex = gridIndex;
        }

        public void SetCard(string id, float lvProgress)
        {
            if (string.IsNullOrEmpty(id))
            {
                _cardName.text = "Empty";
                _button.interactable = false;
            }
            else
            {
                _cardName.text = id;
                _button.interactable = true;
            }

            SetLevelBar(lvProgress);
        }

        public void TogglePopup()
        {
            bool isPopupActive = _popup.activeSelf;
            _controller.TurnAllPopupsOff();
            _popup.SetActive(!isPopupActive);
        }

        private void SetLevelBar(float progress)
        {
            _levelBar.anchorMax = new Vector2(progress, 0);
        }
    }
}
