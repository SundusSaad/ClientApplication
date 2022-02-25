using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class EmployeeDTO
    {
        public long NumberOfEmployees { get; set; }
        public string FirstName { get; set; }   
        public String LastName { get; set; }
        public String JobTitle { get; set; }
    }
}