using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace display
{
    public class CustomerInfo
    {
        public string name { get; set; }
        public string designation { get; set; }
        public string email { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }



        public CustomerInfo(string name, string designation, string email, string workphone, string mobile)
        {
            this.name = name;
            this.designation = designation;
            this.email = email;
            this.workphone = workphone;
            this.mobile = mobile;

        }

        

    }
}
