
    using System;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Item.Model;
using Microsoft.AspNetCore.Mvc;

using OrderService;

namespace OrderService.Services
{
    public class SalesOrderGrpcService : SalesOrders.SalesOrdersBase
    {
        // Implement the gRPC service methods
        public override Task<SalesOrder> CreateOrder(SalesOrder request, ServerCallContext context)
        {
            // Implement the creation logic for Orders
            return base.CreateOrder(request, context);
        }

        public override Task<GetOrderDetailResponse> ReadOrder(GetOrderDetailRequest request, ServerCallContext context)
        {
            // Implement the read logic for Orders
            return base.ReadOrder(request, context);
        }

        public override Task<SalesOrder> UpdateOrder(SalesOrder request, ServerCallContext context)
        {
            // Implement the update logic for Orders
            return base.UpdateOrder(request, context);  
        }

        public override Task<GetOrderDetailResponse> DeleteOrder(GetOrderDetailRequest request, ServerCallContext context)
        {
            return base.DeleteOrder(request, context);
            // Implement the delete logic for Orders
        }

        public override Task<GetOrderResponse> ListOrders(GetOrderRequest request, ServerCallContext context)
        {
            return base.ListOrders(request, context);

            // Implement the listing logic for Orders
        }
    }
  

}