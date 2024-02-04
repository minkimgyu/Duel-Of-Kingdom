using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClientInfo.Tower
{
    public enum TowerType
    {
        king_tower = 1,
        left_princess_tower,
        right_princess_tower,
    }

    public class TowerData
    {
        public TowerType type { get; set; }
        public int level { get; set; }
        public int hp { get; set; }

        public TowerData() {}
        public TowerData(TowerType type, int level, int hp) {
            this.type = type;
            this.level = level;
            this.hp = hp;
        }
    }
}
