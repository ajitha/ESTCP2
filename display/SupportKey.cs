using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace display
{
    public class SupportKey
    {
        public string key { get; set; }
        public string description { get; set; }


        public SupportKey(string key, string description)
        {
            this.key = key;
            if (String.IsNullOrEmpty(description))
            {
                this.description = "";
            }
            else {
                this.description = description;
            }
        }

    }
}
