
namespace WPP.ClashRoyale_Server.Database.ClientInfo.CardData
{
    public enum CardRarity
    {
        Common = 1,
        Rare,
        Epic,
        Legendary,
        Champion,
    }
    class Card
    {
        public int id { get; set; }
        public int level { get; set; }
        public int needElixir { get; set; }
        public CardRarity rarity { get; set; }

        public Card(int id, int level, CardRarity rarity, int needElixir) {
            this.id = id;
            this.level = level;
            this.rarity = rarity;
            this.needElixir = needElixir;
        }
    }
}
