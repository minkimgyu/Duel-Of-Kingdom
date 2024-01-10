using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Database.CardData
{
    // 모든 카드 정보를 저장
    class Cards
    {
        public Card[] cards;

        public Cards()
        {
            cards = new Card[Constants.MAXIMUM_CARDS];
        }
    }
}
