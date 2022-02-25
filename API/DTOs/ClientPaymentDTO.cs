using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ClientPaymentDTO
    {
        public string ClientName { get; set; }
        public long Count { get; set; }
        public IList<String> ClientInvoice { get; set; }
        public IList<double> ClientPayment { get; set; }
        public IList<String> PayMethod { get; set; }
        public int Payment_using_CreditCard { get; set; }
        public int Payment_using_Cash { get; set; }
    }
}