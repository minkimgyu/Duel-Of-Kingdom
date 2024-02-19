using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.GRID;

namespace WPP.DeckManagement
{
    public class Card
    {
        public enum Type
        {
            Troop, Spell, Building
        }
        public enum Rarity
        {
            Common, Rare, Epic, Legendary
        }

        public string id = "Empty"; // = CardData.unit.name
        public Type type;
        public Rarity rarity;

        public List<EntitySpawnData> entities = new();

        public OffsetRect gridSize = new(0,0,0,0);
        public int cost = 0;

        public static Card Empty = new() { id = "Empty" };

        public bool IsEmpty() => id == "Empty";
    }
}
