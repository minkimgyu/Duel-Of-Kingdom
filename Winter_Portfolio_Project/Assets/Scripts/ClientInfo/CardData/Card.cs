using UnityEngine;

namespace WPP.ClientInfo.CardData
{
    public enum CardType
    {
        Troop, Spell, Building
    }
    public enum CardRarity
    {
        Common, Rare, Epic, Legendary
    }

    public class Card
    {
        public int id { get; set; }

        public CardType type { get; set; }

        public CardRarity rarity { get; set; }

        public int needElixir { get; set; }

        public Vector2Int gridSize { get; set; }

        public Card() { }
        public Card(int id, CardType type, CardRarity rarity, int needElixir, Vector2Int gridSize)
        {
            this.id = id;
            this.rarity = rarity;
            this.needElixir = needElixir;
            this.gridSize = gridSize;
        }
    }
}
