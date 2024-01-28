using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Data.Units;

namespace WPP.ClashRoyale_Server.Data.Units
{
    class Troop : Unit
    {
        public float hitpoints { get; set; }
        public float damage { get; set; }
        public float hit_speed { get; set; }
        public float range { get; set; }

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
