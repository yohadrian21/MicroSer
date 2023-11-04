using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroSer.Data;
using MicroSer.model;
using Grpc.Net.Client;
using CustomerClient;

namespace MicroSer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly MicroSerContext _context;

        public CustomersController(MicroSerContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<model.Customer>>> GetCustomer()
        { 
            var channel = GrpcChannel.ForAddress("https://localhost:7296");
            var customerClient = new Customers.CustomersClient(channel);
            var getRequest = new GetCustomerRequest();
            var CustomerReturn = new List<model.Customer>();

            try
            {
                var customerlist = await customerClient.GetCustomerAsync(getRequest);
                if (customerlist.Code != 200)
                {
                    return NotFound();
                }
                foreach (var customer in customerlist.Data)
                {
                    var tempCust = new model.Customer();
                    tempCust.Address = customer.Address;
                    tempCust.Name = customer.Name;
                    tempCust.ID = customer.Id;
                    CustomerReturn.Add(tempCust);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }
            return CustomerReturn;
        }


        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<model.Customer>> GetCustomers(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7296");
            var customerClient = new Customers.CustomersClient(channel);
            var getRequest = new GetCustomerDetailRequest()
            {
                ID = id,
            };
            var CustomerReturn = new model.Customer();

            try
            {
                var customer= await customerClient.GetCustomerDetailAsync(getRequest);
                if (customer.Code != 200)
                {
                    return NotFound();
                }
                CustomerReturn.Address = customer.Data.Address;
                CustomerReturn.Name = customer.Data.Name;
                CustomerReturn.ID = customer.Data.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return CustomerReturn;
         
        }


        // POST api/<InvoicesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] model.Customer customer)
        {
            if (customer == null)
            {
                return BadRequest(); // Return a 400 Bad Request if the customer data is not provided
            }
            var channel = GrpcChannel.ForAddress("https://localhost:7296");
            var customerClient = new Customers.CustomersClient(channel);
            var request = new CustomerClient.Customer();
            try
            {
                request.Address = customer.Address; 
                request.Name=customer.Name;
                var insertCust = await customerClient.CreateCustomerAsync(request);
            }catch (Exception ex) {
                return BadRequest(); // Return a 400 Bad Request if the customer data is not provided
            }

            return Ok();
        }
        private bool CustomerExists(int id)
        {
            return (_context.Customer?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
