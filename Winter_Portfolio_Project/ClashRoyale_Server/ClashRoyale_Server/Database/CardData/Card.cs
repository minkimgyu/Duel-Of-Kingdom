using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Database.CardData
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
        public int level { get; set; }
        public int elixir { get; set; }
        public CardRarity rarity { get; set; }

    }
}
