using Invoices.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace Customer.Data
{
    public class InvoicesContext : DbContext
    {
        public InvoicesContext(DbContextOptions<InvoicesContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Customer { get; set; }
      
        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Data Source=localhost;Initial Catalog=Invoice;Encrypt=false;User Id=sa;Password=Contraseña12345678");
    }
}
