using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Database.ClientInfo.Tower
{
    public enum TowerType
    {
        kingTower = 1,
        princessTower
    }
    public enum TowerStatus
    {
        NOT_DESTROYED = 1,
        DESTROYED
    }
    class Tower
    {
        int[] kingTowerHPByLevel = {
            2400, 2568, 2736, 2904, 3096, 3312, 3528, 3768, 4008, 4392,
            4824, 5304, 5832, 6408, 7032
        };

        int[] princessTowerHPByLevel = {
            1400, 1512, 1624, 1750, 1890, 2030, 2184, 2352, 2534,
            2786, 3052, 3346, 3668, 4032, 4424
        };
    
        public TowerType type { get; set; }

        public TowerStatus status { get; set; }
        public int level { get; set; }
        public int hp { get; set; }

        public Tower(TowerType type, int level)
        {
            this.type = type;
            this.status = TowerStatus.NOT_DESTROYED;
            this.level = level;
            this.hp = (type == TowerType.kingTower) ? kingTowerHPByLevel[level - 1] : princessTowerHPByLevel[level - 1];
        }
    }
}
