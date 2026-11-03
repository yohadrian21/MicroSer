# .NET gRPC Microservices

Three gRPC microservices (Customer, Item, Order) behind an ASP.NET Core REST gateway. Each service owns its database through EF Core.

**Tech:** C# · .NET 6 · ASP.NET Core Web API · gRPC (Grpc.AspNetCore, HTTP/2) · Protocol Buffers · EF Core 7 · SQL Server · Docker

## Architecture
```
              REST (JSON)
 client ───────────────▶  MicroSer (API gateway)
                              │ gRPC
         ┌────────────────────┼────────────────────┐
         ▼                    ▼                    ▼
   CustomerGRPC           ItemGRPC             OrderGRPC
   (CustomerContext)      (ItemContext)        (OrderContext)
         └──────────── SQL Server (EF Core migrations) ───────┘
```

## Services
| Service | Contract | Operations |
|---|---|---|
| CustomerGRPC | `Protos/Customer.proto` | `GetCustomer` (search by name), `GetCustomerDetail`, `CreateCustomer` |
| ItemGRPC | `Protos/Item.proto` | `GetItem` (search by name), `GetItemDetail`, `CreateItem` (name, unit price) |
| OrderGRPC | `Protos/orders.proto` | `CreateOrder`, `ReadOrder`, `UpdateOrder`, `DeleteOrder`, `ListOrders` (orders with line items, subtotal, tax, grand total) |
| MicroSer | REST controllers | `api/Customers`, `api/Item`, `api/Order`; the order detail endpoint also calls CustomerGRPC to enrich the response |

## Run
```bash
# per service: apply migrations, then run
dotnet ef database update --project OrderGRPC
dotnet run --project OrderGRPC
dotnet run --project ItemGRPC
dotnet run --project CustomerGRPC
dotnet run --project MicroSer
```
Each project also has a Dockerfile (Linux target).

## Roadmap
- Move service addresses to configuration and use the gRPC client factory.
- docker-compose for gateway + services + SQL Server.
- Unit tests for the service layer.
