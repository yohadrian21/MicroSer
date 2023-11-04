using Customer.Data;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Customer.model;

namespace CustomerGRPC.Services
{
    public class CustomerService : Customers.CustomersBase
    {
        private readonly CustomerContext _context;

        private readonly ILogger<CustomerService> _logger;

        private bool CustomerExists(int id)
        {
            return (_context.Customer?.Any(e => e.ID == id)).GetValueOrDefault();
        }
        public CustomerService(ILogger<CustomerService> logger, CustomerContext context)
        {
            _logger = logger;
            _context = context;
        }

        public override Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request, ServerCallContext context)
        {
            GetCustomerResponse response = new GetCustomerResponse();
            if (_context.Customer == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            var data = _context.Customer.ToList();
            foreach (var item in data)
            {
                response.Data.Add(new Customer
                {
                    Address = item.Address,
                    Id = item.ID,
                    Name = item.Name
                });
            }
            response.Message = "";
            response.Code = 200;

            return Task.FromResult(response);

        }
        public override Task<GetCustomerDetailResponse>GetCustomerDetail(GetCustomerDetailRequest request, ServerCallContext context)
        {
            GetCustomerDetailResponse response = new GetCustomerDetailResponse();

            if (_context.Customer == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            var customers = _context.Customer.FindAsync(request.ID);

            if (customers == null)
            {
                response.Message = "Data Not Found";
                response.Code = 400;
                return Task.FromResult(response);
            }
            else{
                response.Data = new Customer()
                {
                    Address = customers.Result.Address,
                    Id = customers.Result.ID,
                    Name = customers.Result.Name
                };
                response.Code = 200;
                response.Message = "";
            }

            return Task.FromResult(response);
        }

        public override Task<Customer> CreateCustomer(Customer request, ServerCallContext context)
        {
            try
            {
                _context.Database.BeginTransaction();
                var customer = _context.Customer.Add(new CustomersModel
                {
                    Address =  request.Address,
                    Name = request.Name,
                });
                _context.SaveChanges();
                _context.Database.CommitTransaction();

            }catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
            }

            return Task.FromResult(request);
        }
    }
}