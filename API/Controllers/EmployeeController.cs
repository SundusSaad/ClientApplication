using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IGraphClient _client;

        public EmployeeController(IGraphClient client)
        {
            _client = client;
        }


        [HttpPost("/CreateEmployees")]

        public async Task<ActionResult> CreateEmployees()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///employees.csv"), "line", withHeaders: true)
            .Merge("(employee:Employee{employee_id:toInteger(line.employee_id), first_name:line.first_name, last_name:line.last_name, job_title:line.job_title, salary:line.salary, reports_to:line.reports_to, office_id: line.office_id})")
            .Set(@"employee.first_name = line.first_name, 
                employee.last_name = line.last_name, 
                employee.employee_id =ToInteger(line.employee_id), 
                employee.job_title = line.job_title,
                employee.salary = line.salary,
                employee.reports_to =line.reports_to,
                employee.office_id = line.office_id")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreateOffices")]

        public async Task<ActionResult> CreateOffices()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///offices.csv"), "line", withHeaders: true)
            .Merge("(office:Office{office_id:toInteger(line.office_id), address:line.address, city:line.city, state:line.state})")
            .Set(@"office.office_id =ToInteger(line.office_id),
                office.address = line.address,
                office.city = line.city,
                office.state =line.state")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/GetEmployeesAndOffice")]
        public async Task<ActionResult> GetEmployeesAndOffices()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///employees.csv"), "line", withHeaders: true)
            .Match("(employee: Employee{ employee_id: toInteger(line.employee_id)})")
            .Match("(office: Office{ office_id: toInteger(line.office_id)})")
            .Create("(employee)-[:WORKS_IN]->(office)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/GetEmployeesRelation")]
        public async Task<ActionResult> GetEmployeesRelation()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///employees.csv"), "line", withHeaders: true)
            .Match("(employee: Employee{ employee_id: toInteger(line.employee_id)})")
            .Match("(manager: Employee{ employee_id: toInteger(line.reports_to)})")
            .Create("(employee)-[:REPORTS_TO]->(manager)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        
    }
}