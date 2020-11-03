using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceAccount
{
    class Request
    {
        public string name { get; set; }
        public bool creditonhold { get; set; }
        public string address1_latitude { get; set; }
        public string description { get; set; }
        public string revenue { get; set; }
        public int accountcategorycode { get; set; }

    }
}
