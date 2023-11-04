using Item.Data;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using ItemGRPC;
using Item.Model;

namespace ItemGRPC.Services
{
    public class ItemService : Items.ItemsBase
    {
        private readonly ItemContext _context;

        private readonly ILogger<ItemService> _logger;

        
        public ItemService(ILogger<ItemService> logger, ItemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public override Task<GetItemResponse> GetItem(GetItemRequest request, ServerCallContext context)
        {
            GetItemResponse response = new GetItemResponse();
            if (_context.Items == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            var data = _context.Items.ToList();
            foreach (var item in data)
            {
                response.Data.Add(new Item
                {
                    UnitPrice = (double)item.UnitPrice,
                    Id = item.Id,
                    Name = item.Name
                });
            }
            response.Message = "";
            response.Code = 200;

            return Task.FromResult(response);

        }
        public override Task<GetItemDetailResponse>GetItemDetail(GetItemDetailRequest request, ServerCallContext context)
        {
            GetItemDetailResponse response = new GetItemDetailResponse();

            if (_context.Items == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            var Items = _context.Items.FindAsync(request.ID);

            if (Items == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            else{
                response.Data = new Item()
                {
                    UnitPrice = (double)Items.Result.UnitPrice,
                    Id = Items.Result.Id,
                    Name = Items.Result.Name
                };
                response.Code = 200;
                response.Message = "";
            }

            return Task.FromResult(response);
        }

        public override Task<Item> CreateItem(Item request, ServerCallContext context)
        {
            try
            {
                _context.Database.BeginTransaction();
                var customer = _context.Items.Add(new ItemsModel
                {
                    Name = request.Name,
                    UnitPrice= (decimal)request.UnitPrice,
                });

                _context.SaveChanges();
                _context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
            }

            return base.CreateItem(request, context);
        }
    }
}