using System;
using UnityEngine;
using WPP.Collection;
using WPP.FileReader;
using WPP.Units;

namespace WPP.ClientInfo.Card
{
    public enum CardType
    {
        troop, multiple_troops, attack_building, spawn_building, spell
    }
    public enum CardRarity
    {
        common, rare, epic, legendary
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
        public SpawnData spawnData { get; set; }

        public CardData() { }
        public CardData(int id, CardType type, CardRarity rarity, int needElixir, GridSize gridSize, SpawnData spawnData)
        {
            this.id = id;
            unit = CardCollection.Instance().FindCard(id).unit;
            unit.id = unit_id;
            this.type = type;
            this.rarity = rarity;
            this.needElixir = needElixir;
            this.gridSize = gridSize;
            this.spawnData = spawnData;
        }
    }
}
