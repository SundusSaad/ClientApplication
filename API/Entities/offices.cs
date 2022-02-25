using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class offices
    {
        public int office_id { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
    }
}