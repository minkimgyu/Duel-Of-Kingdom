using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.AI.CAPTURE;

namespace WPP.ClientInfo.Tower
{
    public enum TowerType
    {
        king_tower,
        left_princess_tower,
        right_princess_tower,
    }

    public class TowerData
    {
        public TowerType type;
        public int level;
        public float hitpoints;
        public float damage;
        public float hit_speed;
        public float range;

        public List<CaptureTag> target_tag;

        public TowerData() { }
        public TowerData(TowerType type, int level, float hitpoints, float damage, float hit_speed, float range)
        {
            this.type = type;
            this.level = level;
            this.hitpoints = hitpoints;
            this.damage = damage;
            this.hit_speed = hit_speed;
            this.range = range;
        }
    }
}
