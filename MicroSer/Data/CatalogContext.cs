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

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Data Source=localhost;Initial Catalog=Customer;Encrypt=false;User Id=sa;Password=Contraseña12345678");
    }
}
