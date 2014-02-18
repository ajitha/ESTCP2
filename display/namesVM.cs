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
            names = access.GetNames();
            username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
        private List<string> names;
        private string username;
        Access access = new Access();

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
