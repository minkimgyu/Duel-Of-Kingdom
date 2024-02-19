using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClientInfo.Tower
{
    [Serializable]
    class TowersData
    {
        public TowerData kingTower;
        public TowerData leftPrincessTower;
        public TowerData rightPrincessTower;

        public TowersData() {
            kingTower = new TowerData();
            leftPrincessTower = new TowerData();
            rightPrincessTower = new TowerData();
        }
    }
}
