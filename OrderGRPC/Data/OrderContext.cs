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
    }
}
