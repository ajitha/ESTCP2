using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace display.ViewModel
{
    class infoVM
    {
        public infoVM()
        {
            username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

    }
}
