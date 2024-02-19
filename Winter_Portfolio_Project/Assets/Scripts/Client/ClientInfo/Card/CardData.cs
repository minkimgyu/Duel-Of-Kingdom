using System;
using UnityEngine;
using WPP.Collection;
using WPP.FileReader;
using WPP.AI.STAT;

namespace WPP.ClientInfo.Card
{
    public enum CardType
    {
        troop, building, spell
    }
    public enum CardRarity
    {
        common, rare, epic, legendary
    }

    [Serializable]
    public class CardData
    {
        public int id;
        public int unitID;

        public BaseStat unit;

        public CardType type;

        public CardRarity rarity;

        public int needElixir;

        public float duration = 1.5f;

        public float radius = 0; // �� ���� 0�� �ƴ� ��� ���� ������ �׷������

        public GridSize gridSize;
        public SpawnData spawnData { get; set; }
    }
}