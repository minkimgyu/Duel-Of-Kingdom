using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.Units
{
    class AttackBuildingData : UnitData
    {
        public float hitpoints;
        public float damage;
        public float life_time;
        public float hit_speed;
        public float range;

        public AttackBuildingData(int id, string name, int level)
            : base(id, name, level)
        {
            hitpoints = 0.0f;
            damage = 0.0f;
            life_time = 0.0f;
            hit_speed = 0.0f;
            range = 0.0f;
        }
    }
}
