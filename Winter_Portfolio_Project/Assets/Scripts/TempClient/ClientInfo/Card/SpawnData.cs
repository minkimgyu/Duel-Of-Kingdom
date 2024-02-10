using System;
using UnityEngine;

namespace WPP.ClientInfo.Card
{
    public enum SpawnType
    {
        single_spawn,
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