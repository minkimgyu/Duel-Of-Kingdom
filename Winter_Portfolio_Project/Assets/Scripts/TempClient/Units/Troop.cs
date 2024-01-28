using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WPP.Units
{
    class Troop : Unit
    {
        public float hitpoints;
        public float damage;
        public float hit_speed;
        public float range;

        public Troop(int id, string name, int level)
            : base(id, name, level)
        {
            hitpoints = 0.0f;
            damage = 0.0f;
            hit_speed = 0;
            range = 0;
        }
    }
}
