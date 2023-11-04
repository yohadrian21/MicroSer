using Microsoft.EntityFrameworkCore;
using Order.Data;
using OrderGRPC.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString($"Data Source=localhost;Initial Catalog=Catalog;Encrypt=false;User Id=sa;Password=Contraseña12345678"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<SalesOrderGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
