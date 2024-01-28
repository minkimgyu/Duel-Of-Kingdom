using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Data.Units
{
    class Spell : Unit
    {
        public Spell(int id, string name, int level)
            : base(id, name, level) { }
    }
}
