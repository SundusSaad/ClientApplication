using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ClientInvoicesDTO
    {
        public string ClientName { get; set; }
        public long Count { get; set; }
        public IList<String> ClientInvoice { get; set; }
    }
}