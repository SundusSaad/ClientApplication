using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class payments
    {
        
        public int payment_id { get; set; }
        public int client_id { get; set; }
        public int invoice_id { get; set; }
        public string date { get; set; }
        public double amount { get; set; }
        public string payment_method { get; set; }
    }
}