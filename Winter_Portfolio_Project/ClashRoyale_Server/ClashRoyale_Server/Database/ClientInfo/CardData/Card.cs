
using ClashRoyale_Server.Database.Units;

namespace WPP.ClashRoyale_Server.Database.ClientInfo.CardData
{
    public enum CardType
    {
        Troop, Spell, Building
    }
    public enum CardRarity
    {
        Common, Rare, Epic, Legendary
    }
    public struct Vector2Int
    {
        public int x {get; set;}
        public int y {get; set;}

        public Vector2Int One()
        {
            return new Vector2Int { x = 1, y = 1 };
        }
    }

    class Card
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
