using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Data.Units
{
    public class Unit
    {
        public int id { get; set; }
        public string name { get; set; }
        public int level { get; set; }

        public Unit(int id, string name, int level) 
        {
          this.id = id;
          this.name = name;
          this.level = level;
        }
    }
}
