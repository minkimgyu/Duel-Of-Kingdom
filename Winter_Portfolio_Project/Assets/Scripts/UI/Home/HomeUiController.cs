using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace WPP.UI
{
    public class HomeUiController : MonoBehaviour
    {
        [SerializeField] private RectTransform _battleMenu;
        [SerializeField] private RectTransform _cardsMenu;

        [SerializeField] private float _selectedButtonRatio = 0.7f;
        [SerializeField] private RectTransform _battleMenuButton;
        [SerializeField] private RectTransform _cardMenuButton;

        private void Start()
        {
            _battleMenu.gameObject.SetActive(true);
            _cardsMenu.gameObject.SetActive(true);
            // set the _cardsMenu on left side of the screen
            _cardsMenu.anchoredPosition = new Vector2(-_cardsMenu.rect.width, 0);

            _battleMenuButton.anchorMin = new Vector2(1 - _selectedButtonRatio, 0);
            _battleMenuButton.anchorMax = new Vector2(1, 0);
            _cardMenuButton.anchorMin = new Vector2(0, 0);
            _cardMenuButton.anchorMax = new Vector2(1 - _selectedButtonRatio, 0);
        }

        public void OnBattleButtonClicked()
        {
            _battleMenu.DOAnchorPos(new Vector2(0, 0), 0.5f);
            _cardsMenu.DOAnchorPos(new Vector2(-_cardsMenu.rect.width, 0), 0.5f);

            _battleMenuButton.DOAnchorMin(new Vector2(1 - _selectedButtonRatio, 0), 0.5f);
            _cardMenuButton.DOAnchorMax(new Vector2(1 - _selectedButtonRatio, 0), 0.5f);
        }

        public void OnCardsButtonClicked()
        {
            _battleMenu.DOAnchorPos(new Vector2(_battleMenu.rect.width, 0), 0.5f);
            _cardsMenu.DOAnchorPos(new Vector2(0, 0), 0.5f);

            _battleMenuButton.DOAnchorMin(new Vector2(_selectedButtonRatio, 0), 0.5f);
            _cardMenuButton.DOAnchorMax(new Vector2(_selectedButtonRatio, 0), 0.5f);
        }
    }
}
