using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBFirstDemo.ViewModel
{
    public class EmpViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Profile { get; set; }
        public string Country { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
    }
}