using System;
using WPP.Units;
using WPP.AI.GRID;

namespace WPP.Units
{
    [Serializable]
    internal class BuildingData : UnitData
    {
        public OffsetRect fill_offset;
        public BuildingData(int id, string name, int level)
        : base(id, name, level)
        {
            fill_offset = new OffsetRect();
        }
    }
}