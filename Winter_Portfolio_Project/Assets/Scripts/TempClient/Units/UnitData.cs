using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.Units
{
    public class UnitData
    {
        public int id;
        public string name;
        public int level;

        public UnitData(int id, string name, int level) 
        {
          this.id = id;
          this.name = name;
          this.level = level;
        }
    }
}
