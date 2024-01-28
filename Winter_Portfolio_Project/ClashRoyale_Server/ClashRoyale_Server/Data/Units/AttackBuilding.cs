using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Data.Units
{
    class AttackBuilding : Unit
    {
        public float hitpoints { get; set; }
        public float damage { get; set; }
        public float life_time { get; set; }
        public float hit_speed { get; set; }
        public float range { get; set; }

        public AttackBuilding(int id, string name, int level)
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
