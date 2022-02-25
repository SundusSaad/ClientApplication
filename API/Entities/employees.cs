using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class employees
    {
        public int employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string job_title { get; set; }
        public string salary { get; set; }
        public string reports_to { get; set; }
        public string office_id { get; set; }
    }
}