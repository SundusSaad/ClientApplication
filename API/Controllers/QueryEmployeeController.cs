using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueryEmployeeController : ControllerBase
    {
        private readonly IGraphClient _client;
        public QueryEmployeeController(IGraphClient client)
        {
            _client = client;

        }

        [HttpGet("/GetAllEmployees")]
        public async Task<ActionResult> GetEmployees()
        {
            var emp = await _client.Cypher
            .Match("(employee:Employee)")
            .Return(employee => employee.As<employees>())
            .ResultsAsync;

            return Ok(emp);

        }

        [HttpGet("/GetAllEmployeesWithJobtitle")]
        public async Task<ActionResult> GetEmployeesWithJobTitle()
        {
            IList<EmployeeDTO> _employeedatalist = new List<EmployeeDTO>();
            var employees = await _client.Cypher
            .Match("(employee:Employee)")
            .Return(employee => employee.As<employees>())
            .ResultsAsync;
            foreach(var result in employees)
            {    
                EmployeeDTO _employeedata = new EmployeeDTO();
                _employeedata.FirstName = result.first_name;
                _employeedata.LastName = result.last_name;
                _employeedata.JobTitle = result.job_title;
                _employeedatalist.Add(_employeedata);
            }
            if(_employeedatalist == null)
            {
                return BadRequest();
            }
            else
            {
            
            return Ok(_employeedatalist);

            }

        }

        [HttpGet("/GetEmployeesWithOffice")]
        public async Task<ActionResult> GetEmployeeWithOffice(string UserName)
        { 
            EmployeeOfficeDTO employeeOffice = new EmployeeOfficeDTO();
            var employees = await _client.Cypher
            .Match("(employee:Employee)-[:WORKS_IN]->(office:Office)")
            .Where("employee.first_name =~ $terms OR employee.last_name =~$terms")
            .WithParam("terms", "(?i)" + UserName)   //WithParam("terms", "(?ui)" + UserName +".*")
            .Return((employee, office) => new
            {
                Employee = employee.As<employees>(),
                Office = office.As<offices>(),
            })
            .ResultsAsync;

            foreach(var result in employees){
                employeeOffice.FirstName = result.Employee.first_name;
                employeeOffice.LastName = result.Employee.last_name;
                employeeOffice.OfficeAddress = result.Office.address;
                employeeOffice.City = result.Office.city;
            };
            if(employeeOffice.LastName == null)
            {
                return BadRequest("Invalid Client");
            }
            else
            {
            return Ok(employeeOffice);
            }
        }



    }
}