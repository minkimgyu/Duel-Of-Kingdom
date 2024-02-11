using System;
using WPP.Units;

namespace WPP.Units
{
    [Serializable]
    internal class BuildingData : UnitData
    {
        public struct OffsetRect
        {
            public int up;
            public int down;
            public int left;
            public int right;
        }

        public OffsetRect fill_offset;
        public BuildingData(int id, string name, int level)
        : base(id, name, level)
        {
            fill_offset = new OffsetRect();
        }
    }
}