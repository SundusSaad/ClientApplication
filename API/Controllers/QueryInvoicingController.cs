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

    public class QueryInvoicingController : ControllerBase
    {
        private readonly IGraphClient _client;
        public QueryInvoicingController(IGraphClient client)
        {
            _client = client;

        }

        [HttpGet("/GetAllClients")]
        public async Task<ActionResult> GetClients()
        {
            var clients = await _client.Cypher
            .Match("(client:Client)")
            .Return(client => client.As<client>())
            .ResultsAsync;

            return Ok(clients);

        }

        [HttpGet("/GetAllClientsWithPhone")]
        public async Task<ActionResult> GetClientsWithPhone()
        {
            IList<clientDTO> _clientdatalist = new List<clientDTO>();
            var clients = await _client.Cypher
            .Match("(client:Client)")
            .Return(client => client.As<client>())
            .ResultsAsync;

            foreach(var result in clients)
            {    
                clientDTO _clientdata = new clientDTO();
                _clientdata.name = result.name;
                _clientdata.city = result.city;
                _clientdata.phone = result.phone;
                _clientdatalist.Add(_clientdata);
            }
            if(_clientdatalist == null)
            {
                return BadRequest();
            }
            else
            {
            return Ok(_clientdatalist);
            }

        }

        [HttpGet("/GetClientsWithInvoices")]
        public async Task<ActionResult> GetClientsWithInvoices(string UserName)
        { 
            ClientInvoicesDTO clientInvoices = new ClientInvoicesDTO();
            IList<String> _clientInvoice = new List<String>();
            var client = await _client.Cypher
            .Match("(client:Client)-[:HAS_AN_INVOICE]->(invoice:Invoices)")
            .Where("client.name =~ $terms")
            .WithParam("terms", "(?i)" + UserName)   //WithParam("terms", "(?ui)" + UserName +".*")
            .Return((client, invoice) => new
            {
                Client = client.As<client>().name,
                Invoice = invoice.CollectAs<invoices>(),
                NumberOfInvoices = invoice.Count()
            })
            .ResultsAsync;

            foreach(var result in client){
                foreach (var item in result.Invoice)
                {
                    _clientInvoice.Add(item.number);
                }
                clientInvoices.ClientName = result.Client;
                clientInvoices.Count = result.NumberOfInvoices;
                clientInvoices.ClientInvoice = _clientInvoice;
            };
            if(clientInvoices.ClientName == null)
            {
                return BadRequest("Invalid Client");
            }
            else
            {
            return Ok(clientInvoices);
            }
        }

        [HttpGet("/GetClientsWithPayments")]
        public async Task<ActionResult> GetClientsWithPyments(string UserName)
        { 
            var pay_using_cash = 0;
            var pay_using_card = 0;
            ClientPaymentDTO clientPayments = new ClientPaymentDTO();
            IList<String> _clientInvoice = new List<String>();
            IList<double> _clientPayments = new List<double>();
            IList<String> _clientPayMethods = new List<String>();
            var client = await _client.Cypher
            .Match("(client:Client)-[:HAS_AN_INVOICE]->(invoice:Invoices)-[:HAS_PAYMENT]->(payment:Payment)-[:HAS_MADE_USING]->(payment_method:Payment_Method)")
            .Where("client.name =~ $terms")
            .WithParam("terms", "(?i)" + UserName)
            .Return((client, invoice, payment, payment_method) => new
            {
                Client = client.As<client>().name,
                Invoice = invoice.CollectAs<invoices>(),
                NumberOfInvoices = invoice.Count(),
                Payment = payment.CollectAs<payments>(),
                PaymentMethod = payment_method.CollectAs<paymentMethod>()
            })
            .ResultsAsync;

            foreach(var result in client){
                foreach (var item in result.Invoice)
                {
                    _clientInvoice.Add(item.number);
                }
                foreach (var item in result.Payment)
                {
                    _clientPayments.Add(item.amount);
                }
                foreach (var item in result.PaymentMethod)
                {
                    _clientPayMethods.Add(item.name);
                    if(item.name == "Credit Card"){
                        pay_using_card++; 
                    }
                    if(item.name == "Cash"){
                        pay_using_cash++; 
                    }
                }

                clientPayments.ClientName = result.Client;
                clientPayments.Count = result.NumberOfInvoices;
                clientPayments.ClientInvoice = _clientInvoice;
                clientPayments.ClientPayment = _clientPayments;
                clientPayments.PayMethod = _clientPayMethods;
                clientPayments.Payment_using_CreditCard = pay_using_card;
                clientPayments.Payment_using_Cash = pay_using_cash;
            };
            if(clientPayments.ClientName == null)
            {
                return BadRequest("Invalid Client");
            }
            else
            {
            return Ok(clientPayments);
            }
        }



    }
}