
    using System;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Order.Model;
using Microsoft.AspNetCore.Mvc;
using Order.Data;
using OrderGRPC;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace OrderGRPC.Services
{
    public class SalesOrderGrpcService : SalesOrders.SalesOrdersBase
    {
        private readonly OrderContext _context;

        private readonly ILogger<SalesOrderGrpcService> _logger;


        public SalesOrderGrpcService(ILogger<SalesOrderGrpcService> logger, OrderContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Implement the gRPC service methods
        public override Task<SalesOrder> CreateOrder(SalesOrder request, ServerCallContext context)
        {
            try
            {

                _context.Database.BeginTransaction();
                var insert = new Orders()
                {
                    CustomerID = request.CustomerID,
                    DeliveryDate = request.DeliveryDate.ToDateTime(),
                    IssueDate = request.IssueDate.ToDateTime(),
                    Status = request.Status,
                    Subject = request.Subject,
                    Tax = request.Tax,
                    SubTotal = request.SubTotal,
                    GrandTotal = request.GrandTotal,
                    
                };
                _context.Order.Add(insert);
                _context.SaveChanges();
                foreach (var item in request.OrdersDetail)
                {
                    _context.OrdersDetail.Add(new OrdersDetail()
                    {
                        Amount = item.Amount,
                        ItemID = item.ItemID,
                        OrdersID = insert.ID,
                        Qty = item.Qty,
                    });
                }
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch  (Exception ex)
            {
                _context.Database.RollbackTransaction();
            }

            return Task.FromResult(request);
        }

        public override Task<GetOrderDetailResponse> ReadOrder(GetOrderDetailRequest request, ServerCallContext context)
        {
            GetOrderDetailResponse response = new GetOrderDetailResponse();

            if (_context.Order == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            var Orders = _context.Order.Find(request.ID);

            if (Orders == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            else
            {
                response.Data = new SalesOrder()
                {
                  CustomerID = Orders.CustomerID,
                  DeliveryDate = Timestamp.FromDateTime(DateTime.SpecifyKind(Orders.DeliveryDate, DateTimeKind.Utc)),
                  GrandTotal = Orders.GrandTotal,
                  ID = Orders.ID,
                  IssueDate = Timestamp.FromDateTime(DateTime.SpecifyKind(Orders.IssueDate, DateTimeKind.Utc)),
                  //OrdersNumber = Orders.OrdersNumber,
                  Status = Orders.Status,
                  Subject = Orders.Subject,
                  SubTotal = Orders.SubTotal,    
                  Tax = Orders.Tax,
                  TotalItem = Orders.TotalItem,    
                };


                var tempDetail = _context.OrdersDetail.Where(entity => entity.ID == request.ID).ToList();
                foreach (var item in tempDetail)
                {
                    var tempOrder = new SalesOrderDetail()
                    {
                        ItemID = item.ItemID,
                        Amount = item.Amount,
                        ID = item.ID,
                        OrdersID = item.OrdersID,
                        Qty = item.Qty,
                    };

                    response.Data.OrdersDetail.Add(tempOrder);
                }
                response.Code = 200;
                response.Message = "";

            }

            return Task.FromResult(response);
        }

        public override Task<SalesOrder> UpdateOrder(SalesOrder request, ServerCallContext context)
        {
            // Implement the update logic for Orders
            try
            {
                _context.Database.BeginTransaction();

                var entityToUpdate = _context.Order.Find(request.ID); // Replace YourEntities with the DbSet name
                if (entityToUpdate != null)
                {
                    // Step 2: Modify the properties of the entity
                    entityToUpdate.Status = 2;//status for finish

                    // Step 3: Save the changes to the database
                    _context.SaveChanges();
                }
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();    
            }
            return base.UpdateOrder(request, context);  
        }

        public override Task<GetOrderDetailResponse> DeleteOrder(GetOrderDetailRequest request, ServerCallContext context)
        {
            // Implement the update logic for Orders
            try
            {
                _context.Database.BeginTransaction();

                var entityToUpdate = _context.Order.Find(request.ID); // Replace YourEntities with the DbSet name
                if (entityToUpdate != null)
                {
                    // Step 2: Modify the properties of the entity
                    entityToUpdate.Status = 4;//status for delete
                    // Step 3: Save the changes to the database
                    _context.SaveChanges();
                }
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
            }
            return base.DeleteOrder(request, context);
            // Implement the delete logic for Orders
        }

        public override Task<GetOrderResponse> ListOrders(GetOrderRequest request, ServerCallContext context)
        {

            GetOrderResponse response = new GetOrderResponse();

            if (_context.Order == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            var Orders = _context.Order.Where(entity => entity.Status!=4).ToList();
            foreach (var o in Orders)
            {
                response.Data.Add(new SalesOrder { 
                    CustomerID = o.CustomerID,
                    DeliveryDate = Timestamp.FromDateTime(DateTime.SpecifyKind(o.DeliveryDate, DateTimeKind.Utc)), 
                    GrandTotal =o.GrandTotal,
                    ID = o.ID,  
                    IssueDate   = Timestamp.FromDateTime(DateTime.SpecifyKind(o.IssueDate, DateTimeKind.Utc)),
                    //OrdersNumber = o.OrdersNumber,
                    Status = o.Status,
                    Subject = o.Subject,
                    SubTotal    = o.SubTotal,
                    Tax = o.Tax,
                    TotalItem = o.TotalItem,
                });
            }
            response.Message = "";
            response.Code = 200;
            return Task.FromResult(response);
        }
    }
  

}