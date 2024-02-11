using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.AI.CAPTURE;
using WPP.Units;

namespace WPP.ClientInfo.Tower
{
    public enum TowerType
    {
        king_tower,
        left_princess_tower,
        right_princess_tower,
    }

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
