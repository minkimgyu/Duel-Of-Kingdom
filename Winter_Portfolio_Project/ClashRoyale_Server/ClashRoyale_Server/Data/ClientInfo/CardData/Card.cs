
using WPP.ClashRoyale_Server.Data.Units;
using System;
using System.Text.Json.Serialization;

namespace WPP.ClashRoyale_Server.Data.ClientInfo.CardData
{
    public enum CardType
    {
        troop, attack_building, spawn_building, spell
    }
    public enum CardRarity
    {
        common, rare, epic, legendary
    }

    public struct GridSize
    {
        public int top { get; set; }
        public int down { get; set; }
        public int left { get; set; }
        public int right { get; set; }

        public GridSize TroopGrid()
        {
            GridSize gridSize = new GridSize();
            gridSize.top = 0;
            gridSize.down = 0;
            gridSize.left = 0;
            gridSize.right = 0;
            return gridSize;
        }

        public GridSize BuildingGrid()
        {
            GridSize gridSize = new GridSize();
            gridSize.top = 1;
            gridSize.down = -1;
            gridSize.left = -1;
            gridSize.right = 1;
            return gridSize;
        }
    }
    class Card
    {
        public int id { get; set; }
        public int unit_id { get; set; }

        public Unit unit { get; set; }

        public CardType type { get; set; }

        public CardRarity rarity { get; set; }

        public int needElixir { get; set; }

        public GridSize gridSize { get; set; }

        public Card() { }
        public Card(int id, int unit_id, CardType type, CardRarity rarity, int needElixir, GridSize gridSize)
        {
            this.id = id;
            this.unit_id = unit_id;
            this.type = type;
            unit = DatabaseManager.Instance().FindUnit(type, unit_id);
            this.rarity = rarity;
            this.needElixir = needElixir;
            this.gridSize = gridSize;
        }
    }
}
