
using ClashRoyale_Server.Database;

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
        private int _id;

        private CardRarity _rarity;

        private int _needElixir;

        private int _unitID;

        private Unit _unit;

        public Card() { }
        public Card(int id, int unitID, CardRarity rarity, int needElixir)
        {
            this._id = id;
            this._unitID = unitID;
            this._rarity = rarity;
            this._needElixir = needElixir;
            _unit = DatabaseManager.Instance().FindUnit(unitID);
        }
    }
}
