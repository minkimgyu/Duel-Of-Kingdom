using System;
using UnityEngine;
using WPP.Collection;
using WPP.FileReader;
using WPP.Units;

namespace WPP.ClientInfo.Card
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
    public class CardData
    {
        public int id;
        public int unit_id;

        public UnitData unit;

        public CardType type;

        public CardRarity rarity;

        public int needElixir;

        public GridSize gridSize;

        public CardData() { }
        public CardData(int id, CardType type, CardRarity rarity, int needElixir, GridSize gridSize)
        {
            this.id = id;
            unit = CardCollection.Instance().FindCard(id).unit;
            this.type = type;
            this.rarity = rarity;
            this.needElixir = needElixir;
            this.gridSize = gridSize;
        }
    }
}
