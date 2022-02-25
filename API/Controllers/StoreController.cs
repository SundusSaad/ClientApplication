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
    public class StoreController:ControllerBase
    {
        private readonly IGraphClient _client;
        public StoreController(IGraphClient client)
        {
            _client = client;
        }

        [HttpPost("/CreateCustomers")]

        public async Task<ActionResult> CreateCustomers()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///customers.csv"), "line", withHeaders: true)
            .Merge("(customer:Customer{customer_id:toInteger(line.customer_id), first_name:line.first_name, last_name:line.last_name,birth_date:line.birth_date, phone:line.phone, address:line.address, city:line.city, state:line.state, points:line.points })")
            .Set(@"customer.customer_id = toInteger(line.customer_id),
                customer.first_name = line.first_name, 
                customer.last_name = line.last_name,
                customer.birth_date = line.birth_date, 
                customer.phone = line.phone,
                customer.address = line.address,
                customer.city = line.city,
                customer.state = line.state, 
                customer.points = line.points")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreateProducts")]

        public async Task<ActionResult> CreateProducts()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///products.csv"), "line", withHeaders: true)
            .Merge("(product:Product{product_id:toInteger(line.product_id), name:line.name, quantity_in_stock:line.quantity_in_stock, unit_price:line.unit_price})")
            .Set(@"product.name = line.name,
                product.product_id =ToInteger(line.product_id),
                product.quantity_in_stock = line.quantity_in_stock,
                product.unit_price = ToFloat(line.unit_price)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreateOrderItemNotes")]

        public async Task<ActionResult> CreateOrderItemNotes()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///order_item_notes.csv"), "line", withHeaders: true)
            .Create("(order_item_notes:Order_Item_Notes{note_id:toInteger(line.note_id), order_id:toInteger(line.order_id), product_id:toInteger(line.product_id), note:line.note})")
            .Set(@"order_item_notes.note_id = toInteger(line.note_id),
                order_item_notes.order_id = toInteger(line.order_id),
                order_item_notes.product_id = toInteger(line.product_id),
                order_item_notes.note = line.note")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        
        [HttpPost("/CreateOrderItems")]

        public async Task<ActionResult> CreateOrderItems()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///order_items.csv"), "line", withHeaders: true)
            .Merge("(order_item:Order_Item{order_id:toInteger(line.order_id), product_id:toInteger(line.product_id), quantity:line.quantity, unit_price:line.unit_price})")
            .Set(@"order_item.order_id = line.order_id,
                order_item.product_id = toInteger(line.product_id),
                order_item.quantity = line.quantity, 
                order_item.unit_price = line.unit_price")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreateOrderStatus")]

        public async Task<ActionResult> CreateOrderStatus()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///order_statuses.csv"), "line", withHeaders: true)
            .Merge("(order_status:Order_Status{order_status_id:toInteger(line.order_status_id), name:line.name})")
            .Set(@"order_status.order_status_id =ToInteger(line.order_status_id),
                order_status.name = line.name")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreateOrders")]

        public async Task<ActionResult> CreateOrders()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///orders.csv"), "line", withHeaders: true)
            .Create("(order:Order{order_id:toInteger(line.order_id), customer_id:toInteger(line.customer_id), order_date:line.order_date,status:line.status, comments:line.comments, shipped_date:line.shipped_date, shipper_id:toInteger(line.shipper_id)})")
            .Set(@"order.order_id = toInteger(line.order_id), 
                order.customer_id = toInteger(line.customer_id), 
                order.order_date = line.order_date,
                order.status = line.status, 
                order.comments = line.comments,
                order.shipped_date = line.shipped_date, 
                order.shipper_id = toInteger(line.shipper_id)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpPost("/CreateShippers")]

        public async Task<ActionResult> CreateShippers()
        {

            await _client.Cypher.LoadCsv(new Uri("file:///shippers.csv"), "line", withHeaders: true)
            .Merge("(shipper:Shipper{shipper_id:toInteger(line.shipper_id), name:line.name})")
            .Set(@"shipper.shipper_id =ToInteger(line.shipper_id),
                shipper.name = line.name")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/CustomerOrders")]
        public async Task<ActionResult> GetCustomerOrders()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///orders.csv"), "line", withHeaders: true)
            .Merge("(customer:Customer{customer_id:toInteger(line.customer_id)})")
            .Merge("(order:Order{order_id:toInteger(line.order_id)})")
            .Merge("(customer)-[:HAS_MADE_AN_ORDER]->(order)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }


        
        [HttpGet("/OrdersAndProducts")]
        public async Task<ActionResult> GetOrdersAndProducts()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///order_items.csv"), "line", withHeaders: true)
            .Merge("(product:Product{product_id:toInteger(line.product_id)})")
            .Merge("(order:Order{order_id:toInteger(line.order_id)})")
            .Merge("(order)-[:OF_PRODUCT]->(product)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }


                
        [HttpGet("/OrdersAndShippers")]
        public async Task<ActionResult> GetOrdersAndShippers()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///orders.csv"), "line", withHeaders: true)
            .Match("(order:Order{order_id:toInteger(line.order_id)})")
            .Match("(shipper:Shipper{shipper_id:toInteger(line.shipper_id)})")
            .Merge("(shipper)-[:HAS_SHIPPED]->(order)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/OrderAndOrderStatus")]
        public async Task<ActionResult> GetOrderAndOrderStatus()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///orders.csv"), "line", withHeaders: true)
            .Merge("(order:Order{order_id:toInteger(line.order_id)})")
            .Merge("(order_status:Order_Status{order_status_id:toInteger(line.status)})")
            .Merge("(order)-[:HAS_STATUS]->(order_status)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        
        [HttpGet("/OrderAndNotes")]
        public async Task<ActionResult> GetOrderAndNotes()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///order_item_notes.csv"), "line", withHeaders: true)
            .Merge("(order_item_notes:Order_Item_Notes{note_id:toInteger(line.note_id)})")
            .Merge("(order:Order{order_id:toInteger(line.order_Id)})")
            .Merge("(order)-[:HAS_NOTES]->(order_item_notes)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpGet("/OrderItemsAndProducts")]
        public async Task<ActionResult> GetOrderItemsAndProducts()
        {
            await _client.Cypher.LoadCsv(new Uri("file:///order_items.csv"), "line", withHeaders: true)
            .Merge("(product:Product{product_id:toInteger(line.product_id)})")
            .Merge("(order_item:Order_Item{quantity:line.quantity})")
            .Merge("(product)-[:IN_QUANTITY]->(order_item)")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpDelete("/orderItemAndRelation")]
        public async Task<ActionResult> DeleteOrderItem()
        {
            await _client.Cypher
            .OptionalMatch("(order_item:Order_Item)-[r]-()")
            .DetachDelete("order_item, r")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }




    }
}