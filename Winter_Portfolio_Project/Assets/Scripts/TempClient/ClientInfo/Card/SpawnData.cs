using System;
using UnityEngine;

namespace WPP.ClientInfo.Card
{
    public enum SpawnType
    {
        single_spawn = 1,
        triple_spawn
    }

    [Serializable]
    public class SpawnData
    {
        public SpawnType type;
        public int spawnUnitCount;
        public Vector2[] spawnOffset;
    }
}