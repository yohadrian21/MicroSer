using Order.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Order.Model;
using System.Reflection.Metadata;

namespace Order.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Orders> Order { get; set; }

        public DbSet<OrdersDetail> OrdersDetail { get; set; }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Data Source=localhost;Initial Catalog=Order;Encrypt=false;User Id=sa;Password=Contraseña12345678");
    }
}
