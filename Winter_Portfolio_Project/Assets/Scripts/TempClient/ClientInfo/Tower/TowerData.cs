using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.AI.CAPTURE;
using WPP.Units;

namespace WPP.ClientInfo.Tower
{
    class TowerData
    {
        public AttackBuildingData towerUnit;

        public TowerData()
        {
            towerUnit = null;
        }

        public TowerData(AttackBuildingData tower)
        {
            towerUnit = tower;
        }
    }
}
