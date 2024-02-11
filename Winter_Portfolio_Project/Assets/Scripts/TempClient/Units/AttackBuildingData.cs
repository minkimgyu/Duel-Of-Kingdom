using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.AI.CAPTURE;

namespace WPP.Units
{
    class AttackBuildingData : BuildingData
    {
        public float hitpoints;
        public float damage;
        public float? life_time;
        public float hit_speed;
        public float range;

        public List<CaptureTag> target_tag;

        public AttackBuildingData(int id, string name, int level, float hitpoints = 0.0f, float damage = 0.0f, float? life_time = 0.0f, float hit_speed = 0.0f, float range = 0.0f)
            : base(id, name, level)
        {
            this.hitpoints = hitpoints;
            this.damage = damage;
            this.life_time = life_time;
            this.hit_speed = hit_speed;
            this.range = range;
            target_tag = new List<CaptureTag>();
        }
    }
}
