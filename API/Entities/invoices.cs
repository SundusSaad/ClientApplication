using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class invoices
    {
        public int invoice_id { get; set; }
        public string number { get; set; }
        public int client_id { get; set; }
        public double invoice_total { get; set; }
        public double payment_total { get; set; }
        public string invoice_date { get; set; }
       public string due_date { get; set; }
       public string payment_date { get; set; }
    }
}