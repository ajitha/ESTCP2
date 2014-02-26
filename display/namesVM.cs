using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using display.Model;

namespace display.ViewModel
{
    class namesVM
    {
        public namesVM()
        {
            access = new Access("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=profiles.MDB");
            names = access.GetNames();

            access = new Access("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ESTProfiles.MDB");

            username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
        private List<string> names;
        private string username;
        Access access;
        //Access access = new Access("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=profiles.MDB");

        public List<string> Names
        {
            get { return names; }
            set { names = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
    }
}
