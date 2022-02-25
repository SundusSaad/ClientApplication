using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class EmployeeOfficeDTO
    {
        public string FirstName { get; set; }   
        public String LastName { get; set; }
        public String OfficeAddress { get; set; }
        public string City { get; set; }
    }
}