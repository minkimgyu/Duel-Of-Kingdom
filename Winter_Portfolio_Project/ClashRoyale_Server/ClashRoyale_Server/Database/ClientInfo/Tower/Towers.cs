using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Database.ClientInfo;

namespace WPP.ClashRoyale_Server.Database.ClientInfo.Tower
{
    class Towers
    {
        public List<Tower> towers {  get; set; } 
        public Tower kingTower { get; set; }
        public Tower leftPrincessTower { get; set; }
        public Tower rightPrincessTower { get; set; }

        public int numOfTowersDestroyed;

        public Towers() {
            towers = new List<Tower>();

            kingTower = new Tower(TowerType.kingTower, 1);
            leftPrincessTower = new Tower(TowerType.princessTower, 1);
            rightPrincessTower = new Tower(TowerType.princessTower, 1);

            towers.Add(kingTower);
            towers.Add(leftPrincessTower);
            towers.Add(rightPrincessTower);

            numOfTowersDestroyed = 0;
        }
    }
}
