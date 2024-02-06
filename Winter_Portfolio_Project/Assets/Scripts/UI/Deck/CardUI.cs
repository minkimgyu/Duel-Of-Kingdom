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


        public void Initialize(DeckUIController controller, GameObject popup, int gridIndex)
        {
            _controller = controller;
            _popup = popup;
            _gridIndex = gridIndex;
        }

        public void SetCard(string id, int cost, float lvProgress = 0)
        {
            if (string.IsNullOrEmpty(id))
            {
                _cardName.text = "Empty";
                _button.interactable = false;
                _cardCost.text = "";
            }
            else
            {
                _cardName.text = id;
                _button.interactable = true;
                _cardCost.text = cost.ToString();
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
