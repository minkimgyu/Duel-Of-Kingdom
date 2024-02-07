using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WPP.DeckManagement.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _cardName;
        [SerializeField] private TextMeshProUGUI _cardCost;
        [SerializeField] private Button _button;
        [Header("Optional")]
        [SerializeField] private RectTransform _levelBar;

        private DeckEditorUIController _controller;
        private GameObject _popup;
        private int _gridIndex;


        public void Initialize(DeckEditorUIController controller, GameObject popup, int gridIndex)
        {
            _controller = controller;
            _popup = popup;
            _gridIndex = gridIndex;
        }

        public void SetCard(Card card, float lvProgress = 0)
        {
            if (card.IsEmpty())
            {
                _cardName.text = "Empty";
                _button.interactable = false;
                _cardCost.text = "";
            }
            else
            {
                _cardName.text = card.id;
                _button.interactable = true;
                _cardCost.text = card.cost.ToString();
            }

            if(_levelBar != null)
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
