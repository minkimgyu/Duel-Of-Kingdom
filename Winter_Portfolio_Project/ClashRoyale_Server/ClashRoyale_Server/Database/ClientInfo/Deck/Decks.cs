using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server;

namespace WPP.ClashRoyale_Server.Database.ClientInfo.Deck
{
    class Decks
    {
        public Deck[] decks;
        public Decks()
        {
            decks = new Deck[Constants.MAXIMUM_DECK];
        }
    }
}
