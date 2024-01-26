using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyale_Server.Database.Units
{
    public enum UnitType
    {
        kinght = 1,
        dragon,
        shooter
    }
    class Unit
    {

        private int _id;
        private UnitType _type;
        private int _level;

        public Unit() { }

        public Unit(int id, UnitType type, int level) {
            this._id = id;
            this._type = type;
            this._level = level;
        }
    }
}
