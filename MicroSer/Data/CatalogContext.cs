using MicroSer.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace MicroSer.Data
{
    public class MicroSerContext : DbContext
    {
        public MicroSerContext(DbContextOptions<MicroSerContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<Item> Item { get; set; }
    }
}
