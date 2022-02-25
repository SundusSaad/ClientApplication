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
    public class InvoicingController : ControllerBase
    {
        private readonly IGraphClient _client;
        public InvoicingController(IGraphClient client)
        {
            _client = client;
        }


        [HttpPost("/CreateClients")]

        public async Task<ActionResult> CreateClients()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///clients.csv"), "line", withHeaders: true)
            .Merge("(client:Client{client_id:toInteger(line.client_id), name:line.name, address:line.address, city:line.city, state:line.state, phone:line.phone})")
            .Set(@"client.name = line.name,
                client.address = line.address,
                client.client_id =ToInteger(line.client_id),
                client.city = line.city,
                client.state = line.state,
                client.phone =line.phone")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreateInvoices")]

        public async Task<ActionResult> CreateInvoices()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///invoices.csv"), "line", withHeaders: true)
            .Merge("(invoice:Invoices{invoice_id:toInteger(line.invoice_id), number:line.number, client_id:toInteger(line.client_id),invoice_total:line.invoice_total, payment_total:line.payment_total, invoice_date:line.invoice_date,due_date: line.due_date, payment_date:line.payment_date})")
            .Set(@"invoice.invoice_id = ToInteger(line.invoice_id),
                invoice.number = line.number,
                invoice.client_id =ToInteger(line.client_id),
                invoice.invoice_total = line.line.invoice_total,
                invoice.payment_total = line.payment_total,
                invoice.invoice_date =line.invoice_date,
                invoice.due_date = line.due_date,
                invoice.payment_date = line.payment_date")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreatePaymentMethods")]

        public async Task<ActionResult> CreatePaymentMethods()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///payment_methods.csv"), "line", withHeaders: true)
            .Merge("(payment_method:Payment_Method{payment_method_id:toInteger(line.payment_method_id), name:line.name})")
            .Set(@"payment_method.payment_method_id =ToInteger(line.payment_method_id),
                payment_method.name = line.name")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }


        [HttpPost("/Createpayments")]

        public async Task<ActionResult> CreatePayments()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///payments.csv"), "line", withHeaders: true)
            .Merge("(payment:Payment{payment_id:toInteger(line.payment_id), client_id:toInteger(line.client_id), invoice_id:toInteger(line.invoice_id), date:line.date,amount:line.amount, payment_method:line.payment_method})")
            .Set(@"payment.payment_id =ToInteger(line.payment_id),
                payment.client_id =ToInteger(line.client_id),
                payment.invoice_id =ToInteger(line.invoice_id),
                payment.date = line.date,
                payment.amount = line.amount,
                payment.payment_method = line.payment_method")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/InvoiceOfClients")]
        public async Task<ActionResult> GetInvoiceOfClients()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///invoices.csv"), "line", withHeaders: true)
            .Match("(client:Client{client_id:toInteger(line.client_id)})")
            .Match("(invoice:Invoices{invoice_id:toInteger(line.invoice_id)})")
            .Create("(client)-[:HAS_AN_INVOICE]->(invoice)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/InvoicePayments")]
        public async Task<ActionResult> GetInvoicePayments()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///payments.csv"), "line", withHeaders: true)
            .Match("(client:Client{client_id:toInteger(line.client_id)})")
            .Match("(invoice:Invoices{invoice_id:toInteger(line.invoice_id)})")
            .Match("(payment:Payment{payment_id:toInteger(line.payment_id)})")
            .Match("(payment_method:Payment_Method{payment_method_id:toInteger(line.payment_method)})")
            .Create("(invoice)-[:HAS_PAYMENT]->(payment)-[:HAS_MADE_USING]->(payment_method)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/InvoicePaymentsOfClients")]

        public async Task<ActionResult> GetInvoicePaymentsOfClients()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///payments.csv"), "line", withHeaders: true)
            .Match("(client:Client{client_id:toInteger(line.client_id)})")
            .Match("(invoice:Invoices{invoice_id:toInteger(line.invoice_id)})")
            .Match("(payment:Payment{payment_id:toInteger(line.payment_id)})")
            .Match("(payment_method:Payment_Method{payment_method_id:toInteger(line.payment_method)})")
            .Create("(client)-[:HAS_AN_INVOICE]->(invoice)-[:HAS_PAYMENT]->(payment)-[:HAS_MADE_USING]->(payment_method)")
            .Return((client, invoice, payment, payment_method) => new
            {
                Client = client.As<client>(),
                Invoice = invoice.As<invoices>(),
                Payment = payment.As<payments>(),
                Payment_Method = payment_method.CollectAs<paymentMethod>(),

            })
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

    }
}