using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.DeckManagement
{
    [CreateAssetMenu(fileName = "CardSpriteSO", menuName = "Deck/CardSpriteSO")]
    public class CardSpriteSO : ScriptableObject
    {
        [System.Serializable]
        private class CardSprite
        {
            public string name;
            public Sprite sprite;
        }
        [SerializeField] private Sprite _emptySprite;
        [SerializeField] private Sprite _notFoundSprite;
        [SerializeField] private List<CardSprite> _cardSprites;

        private Dictionary<string, Sprite> _cardSpriteDict;

        private void OnEnable()
        {
            _cardSpriteDict = new Dictionary<string, Sprite>();
            foreach (var cardSprite in _cardSprites)
            {
                _cardSpriteDict.Add(cardSprite.name, cardSprite.sprite);
            }
        }

        public Sprite EmptySprite => _emptySprite;

        public Sprite GetCardSprite(string cardName)
        {
            if (_cardSpriteDict.TryGetValue(cardName, out var sprite))
            {
                return sprite;
            }
            return _notFoundSprite;
        }
    }
}
