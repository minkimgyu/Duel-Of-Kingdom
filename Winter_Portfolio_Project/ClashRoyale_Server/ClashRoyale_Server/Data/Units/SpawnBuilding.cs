using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WPP.ClashRoyale_Server.Data.Units
{
    class SpawnBuilding : Unit
    {
        public float hitpoints { get; set; }
        public float life_time { get; set; }
        public int spawn_unit_id { get; set; }
        public int spawn_unit_count { get; set; }

        public SpawnBuilding(int id, string name, int level)
            : base(id, name, level)
        {
            hitpoints = 0.0f;
            life_time = 0.0f;
            spawn_unit_id = 0;
            spawn_unit_count = 0;
        }
    }
}
