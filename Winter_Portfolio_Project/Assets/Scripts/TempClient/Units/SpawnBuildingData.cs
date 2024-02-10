using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WPP.Units
{
    class SpawnBuildingData : UnitData
    {
        public float hitpoints;
        public float life_time;
        public int spawn_unit_id;
        public int spawn_unit_count;
        public float spawn_delay;
        Vector3Int[] spawnOffset;

        public SpawnBuildingData(int id, string name, int level)
            : base(id, name, level)
        {
            hitpoints = 0.0f;
            life_time = 0.0f;
            spawn_unit_id = 0;
            spawn_unit_count = 0;
            spawn_delay = 0.0f;
            spawnOffset = new Vector3Int[3];
        }
    }
}
