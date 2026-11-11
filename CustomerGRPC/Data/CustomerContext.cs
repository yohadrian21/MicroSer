using Customer.model;
using Microsoft.EntityFrameworkCore;

namespace Customer.Data
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }

        public DbSet<CustomersModel> Customer { get; set; }
    }
}
