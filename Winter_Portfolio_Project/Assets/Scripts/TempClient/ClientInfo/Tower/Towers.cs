using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClientInfo.Tower
{
    public class Towers
    {
        public Tower kingTower { get; set; }
        public Tower leftPrincessTower { get; set; }
        public Tower rightPrincessTower { get; set; }

        public int numOfTowersDestroyed { get; set; }

        public Towers() {
            kingTower = new Tower();
            leftPrincessTower = new Tower();
            rightPrincessTower = new Tower();

            numOfTowersDestroyed = 0;
        }
    }
}
