using Grpc.Net.Client;
using ItemGRPC;
using MicroSer.model;
using Microsoft.AspNetCore.Mvc;
using OrderGRPC;
using static ItemGRPC.Items;

using Google.Protobuf.WellKnownTypes;
using CustomerClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MicroSer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // GET: api/<InvoicesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<model.Orders>>> GetAsync()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7020");
            var OrderClient = new SalesOrders.SalesOrdersClient(channel);
            var getRequest = new GetOrderRequest();
            var OrderReturn = new List<model.Orders>();

            try
            {
                var OrderList = await OrderClient.ListOrdersAsync(getRequest);
                if (OrderList.Code != 200)
                {
                    return NotFound();
                }
                foreach (var Item in OrderList.Data)
                {
                    var tempCust = new model.Orders();

                    OrderReturn.Add(new Orders()
                    {
                        ID = Item.ID,
                        CustomerID = Item.CustomerID,
                        DeliveryDate  =Item.DeliveryDate.ToDateTime(),
                        GrandTotal= Item.GrandTotal,
                        IssueDate= Item.IssueDate.ToDateTime(),
                        //OrdersDetail= Item.OrdersDetail,
                        OrdersNumber= Item.OrdersNumber,
                        Status = Item.Status,
                        Subject= Item.Subject,
                        SubTotal= Item.SubTotal,
                        Tax= Item.Tax,
                        
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return OrderReturn;
        }

        // GET api/<InvoicesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Orders>> GetAsync(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7020");
            var OrderClient = new SalesOrders.SalesOrdersClient(channel);
            var getRequest = new GetOrderDetailRequest() { 
            ID=id};
            var OrderReturn = new model.Orders();

            try
            {
                var OrderList = await OrderClient.ReadOrderAsync(getRequest);
                if (OrderList.Code != 200)
                {
                    return NotFound();
                }
                OrderReturn = new model.Orders()
                {
                    ID = OrderList.Data.ID,
                    CustomerID = OrderList.Data.CustomerID,
                    DeliveryDate = OrderList.Data.DeliveryDate.ToDateTime(),
                    GrandTotal = OrderList.Data.GrandTotal,
                    IssueDate = OrderList.Data.IssueDate.ToDateTime(),
                    //OrdersDetail= Item.OrdersDetail,
                    OrdersNumber = OrderList.Data.OrdersNumber,
                    Status = OrderList.Data.Status,
                    Subject = OrderList.Data.Subject,
                    SubTotal = OrderList.Data.SubTotal,
                    Tax = OrderList.Data.Tax,

                };

                //get customer
                var custchannel = GrpcChannel.ForAddress("https://localhost:7296");
                var customerClient = new Customers.CustomersClient(custchannel);
                var getcustRequest = new GetCustomerDetailRequest()
                {
                    ID = OrderReturn.CustomerID,
                };
                
                try
                {
                    var customer = await customerClient.GetCustomerDetailAsync(getcustRequest);
                    if (customer.Code != 200)
                    {
                        return NotFound();
                    }

                    OrderReturn.Customer = new model.Customer()
                    {
                        Address = customer.Data.Address,
                        Name = customer.Data.Name,
                        ID = customer.Data.Id,
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                foreach (var order in OrderList.Data.OrdersDetail) {
                    //get item
                    var itemchannel = GrpcChannel.ForAddress("https://localhost:7132");
                    var ItemClient = new Items.ItemsClient(itemchannel);
                    var getitemRequest = new GetItemDetailRequest()
                    {
                        ID = order.ItemID,
                    };
                    
                    try
                    {
                        OrderReturn.OrdersDetail = new List<OrdersDetail>();
                        var Item = await ItemClient.GetItemDetailAsync(getitemRequest);
                        if (Item.Code != 200)
                        {
                            return NotFound();
                        }
                        OrderReturn.OrdersDetail.Add(new OrdersDetail
                        {
                            Amount = order.Amount,
                            ItemID = order.ItemID,
                            Qty = order.Qty,
                            OrdersID = order.OrdersID,
                            Item = new model.Item
                            {
                                Id = Item.Data.Id,
                                Name = Item.Data.Name,
                                UnitPrice = (decimal)Item.Data.UnitPrice,

                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return OrderReturn;
        }

        // POST api/<InvoicesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateInvoice payload)
        {
            if (payload == null)
            {
                return BadRequest(); // Return a 400 Bad Request if the customer data is not provided
            }
            var channel = GrpcChannel.ForAddress("https://localhost:7020");
            var client = new SalesOrders.SalesOrdersClient(channel);
            var request = new SalesOrder();

            decimal subtotal = 0, grandtotal = 0, tax = 0;
            var detail = new List<OrderGRPC.SalesOrderDetail>();
            try
            {
                foreach (var item in payload.OrdersDetails)
                {
                    var itemchannel = GrpcChannel.ForAddress("https://localhost:7132");
                    var itemclient = new Items.ItemsClient(itemchannel);
                    var getRequest = new GetItemDetailRequest()
                    {
                        ID = item.ItemID,
                    };
                    var ItemReturn = new model.Item();

                    try
                    {
                        var Item = await itemclient.GetItemDetailAsync(getRequest);
                        if (Item.Code != 200)
                        {
                            return NotFound();
                        }
                        ItemReturn.UnitPrice = (decimal)Item.Data.UnitPrice;
                        ItemReturn.Name = Item.Data.Name;
                        ItemReturn.Id = Item.Data.Id;
                        decimal Amount = 0;
                        Amount = item.Qty * ItemReturn.UnitPrice;
                        subtotal += Amount;
                        detail.Add(new SalesOrderDetail()
                        {
                            ItemID = item.ItemID,
                            Amount = item.Amount,
                            Qty = item.Qty,
                        });
                    }
                    catch  (Exception ex)
                    {

                    }

                }
                grandtotal = (decimal)1.1 * subtotal;
                tax = (decimal)0.1 * subtotal;
                request = new SalesOrder() { 
                    CustomerID = payload.CustomerID,
                    DeliveryDate = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(7)),
                    GrandTotal = (double)grandtotal,
                    IssueDate = Timestamp.FromDateTime(DateTime.UtcNow),
                    Status = 1,
                    SubTotal = (double)subtotal,
                    Tax = (double)tax,
                };
                request.OrdersDetail.AddRange(detail);
                var insertCust = await client.CreateOrderAsync(request);
            }
            catch (Exception ex)
            {
                return BadRequest(); // Return a 400 Bad Request if the customer data is not provided
            }

            return Ok();
        }

        // PUT api/<InvoicesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7020");
            var OrderClient = new SalesOrders.SalesOrdersClient(channel);
        
            try
            {
                var OrderList = await OrderClient.UpdateOrderAsync(new SalesOrder { ID=id});
                
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
        // DELETE api/<TEScontol>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7020");
            var OrderClient = new SalesOrders.SalesOrdersClient(channel);
            var getRequest = new GetOrderDetailRequest()
            {
                ID = id
            };
            var OrderReturn = new model.Orders();

            try
            {
                var OrderList = await OrderClient.DeleteOrderAsync(getRequest);
                if (OrderList.Code != 200)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
    }
}
