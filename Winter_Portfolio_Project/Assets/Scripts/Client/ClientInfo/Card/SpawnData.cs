using System;
using UnityEngine;
using WPP.AI.STAT;

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
        public SerializableVector2[] spawnOffset;
    }
}