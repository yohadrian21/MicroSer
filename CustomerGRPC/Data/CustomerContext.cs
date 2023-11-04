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

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Data Source=localhost;Initial Catalog=Customer;Encrypt=false;User Id=sa;Password=Contraseña12345678");
    }
}
