using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClientInfo.Account
{
    [Serializable]
    public class ClientAccount
    {
        public string username { get; set; }
        public string password { get; set; }
        public int gold { get; set; }
        public int level { get; set; }
        public int exp {  get; set; }
        public int trophy {  get; set; }

        public ClientAccount() { }
    }
}
