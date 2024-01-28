using System;
using UnityEngine;
using WPP.FileReader;
using WPP.Units;

namespace WPP.ClientInfo.CardData
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

    [Serializable]
    public class Card
    {
        public int id;
        public int unit_id;

        public Unit unit;

        public CardType type;

        public CardRarity rarity;

        public int needElixir;

        public GridSize gridSize;

        public Card() { }
        public Card(int id, int unit_id, CardType type, CardRarity rarity, int needElixir, GridSize gridSize)
        {
            this.id = id;
            unit = JsonParser.Instance().FindUnit(id);
            this.rarity = rarity;
            this.needElixir = needElixir;
            this.gridSize = gridSize;
        }
    }
}
