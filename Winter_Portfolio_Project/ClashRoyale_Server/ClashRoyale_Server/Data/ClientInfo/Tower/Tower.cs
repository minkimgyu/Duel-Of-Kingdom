using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Data.ClientInfo.Tower
{
    public enum TowerType
    {
        king_tower = 1,
        left_princess_tower,
        right_princess_tower
    }
    public enum TowerStatus
    {
        NOT_DESTROYED = 1,
        DESTROYED
    }
    class Tower
    {
        public TowerType type { get; set; }

        public TowerStatus status { get; set; }
        public int level { get; set; }
        public int hp { get; set; }

        public Tower() {}
        public Tower(TowerType type, int level, int hp) {
            this.type = type;
            this.level = level;
            this.hp = hp;
        }
    }
}
