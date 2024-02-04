using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClientInfo.Account
{
    [Serializable]
    public class AccountData
    {
        public string username;
        public string password;
        public int gold;
        public int level;
        public int exp;
        public int trophy;

        public AccountData() { }
    }
}
