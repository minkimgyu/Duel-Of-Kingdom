using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.AI.CAPTURE;
using WPP.AI.STAT;

namespace WPP.ClientInfo.Tower
{
    class TowerData
    {
        public AttackBuildingStat towerUnit;

        public TowerData()
        {
            towerUnit = null;
        }

        public TowerData(AttackBuildingStat tower)
        {
            towerUnit = tower;
        }
    }
}
