using ItemGRPC.Services;
using Item.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
var connectionString = builder.Configuration.GetConnectionString("ItemDb")
    ?? throw new InvalidOperationException(
        "Connection string 'ItemDb' is not configured. Set it with 'dotnet user-secrets' or the " +
        "ConnectionStrings__ItemDb environment variable (see README.md).");
builder.Services.AddDbContext<ItemContext>(options =>
{
    options.UseSqlServer(connectionString);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ItemService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
