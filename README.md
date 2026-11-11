# .NET gRPC Microservices

> [!WARNING]
> **Local learning sample. Do not deploy.** Nothing in this repository authenticates or authorizes
> callers: the REST gateway has no `[Authorize]` endpoints and the gRPC services accept any client.
> Anyone who can reach these ports can read, create, modify and delete customers, items and orders.
> Service addresses are also hardcoded to `localhost`. Run it only on your own machine, and do not
> expose it to a network or deploy it until authentication has been added (for example JWT bearer
> authentication with `[Authorize]` on the gateway, plus mutual TLS or tokens between the gateway and
> the gRPC services).

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

## Database configuration
No credentials are stored in this repository. Each project reads a named connection string from
configuration and refuses to start if it is missing:

| Project | Connection string | Database | Login (see `sql/create-app-logins.sql`) | Permissions |
|---|---|---|---|---|
| CustomerGRPC | `CustomerDb` | `Customer` | `customer_app` | read/write |
| ItemGRPC | `ItemDb` | `Catalog` | `item_app` | read/write |
| OrderGRPC | `OrderDb` | `Order` | `order_app` | read/write |
| MicroSer | `MicroSerDb` | `Customer` | `gateway_app` | read-only |

Supply them with [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) for
local development, or with environment variables such as `ConnectionStrings__CustomerDb` (for example
in a container, where user secrets are not available). Never put a connection string containing a
password in `appsettings*.json` or anywhere else in the repository.

The applications must never connect as `sa` or any other administrator. Use an administrator only for
the one-off schema steps below, and give each service its own login that can only read and write data
in its own database.

**Encryption:** connection strings must use `Encrypt=True`. The only exception is local development
against a SQL Server on your own machine with a self-signed certificate (such as the SQL Server
container), where you may add `TrustServerCertificate=True`. Traffic is still encrypted, but the
server's identity is not verified, so never use that setting for a remote or shared server; give the
server a trusted certificate instead. Do not use `Encrypt=False`.

> If you previously ran this project with the `sa` password that used to be hardcoded in this
> repository, treat that password as public and change it.

### Local setup
1. Start SQL Server 2019 or later locally with an administrator password of your own choosing, and
   export it as `SQL_ADMIN_PASSWORD` for step 3. Do not commit it.
2. Choose a password for each application login and store the connection strings as user secrets.
   The values below are placeholders:
   ```bash
   dotnet user-secrets set --project CustomerGRPC "ConnectionStrings:CustomerDb" "Server=localhost;Database=Customer;User Id=customer_app;Password=<customer-app-password>;Encrypt=True;TrustServerCertificate=True"
   dotnet user-secrets set --project ItemGRPC     "ConnectionStrings:ItemDb"     "Server=localhost;Database=Catalog;User Id=item_app;Password=<item-app-password>;Encrypt=True;TrustServerCertificate=True"
   dotnet user-secrets set --project OrderGRPC    "ConnectionStrings:OrderDb"    "Server=localhost;Database=Order;User Id=order_app;Password=<order-app-password>;Encrypt=True;TrustServerCertificate=True"
   dotnet user-secrets set --project MicroSer     "ConnectionStrings:MicroSerDb" "Server=localhost;Database=Customer;User Id=gateway_app;Password=<gateway-app-password>;Encrypt=True;TrustServerCertificate=True"
   ```
3. Create the databases by applying the migrations as an administrator. `--connection` overrides the
   configured connection string for this command only:
   ```bash
   ADMIN="Server=localhost;User Id=<admin-login>;Password=$SQL_ADMIN_PASSWORD;Encrypt=True;TrustServerCertificate=True"
   dotnet ef database update --project CustomerGRPC --connection "$ADMIN;Database=Customer"
   dotnet ef database update --project ItemGRPC     --connection "$ADMIN;Database=Catalog"
   dotnet ef database update --project OrderGRPC    --connection "$ADMIN;Database=Order"
   ```
4. Create the least-privilege logins with the same passwords you used in step 2. `sqlcmd` prompts for
   the administrator password:
   ```bash
   export CUSTOMER_APP_PASSWORD='<customer-app-password>' ITEM_APP_PASSWORD='<item-app-password>' \
          ORDER_APP_PASSWORD='<order-app-password>' GATEWAY_APP_PASSWORD='<gateway-app-password>'
   sqlcmd -S localhost -U <admin-login> -C -i sql/create-app-logins.sql
   ```
   (`-C` trusts the server certificate, so the local-development caveat above applies.)

## Run
```bash
dotnet run --project OrderGRPC
dotnet run --project ItemGRPC
dotnet run --project CustomerGRPC
dotnet run --project MicroSer
```
Each project also has a Dockerfile (Linux target). Pass the connection string to a container as an
environment variable, for example `-e ConnectionStrings__OrderDb=...`.

## Roadmap
- Move service addresses to configuration and use the gRPC client factory.
- docker-compose for gateway + services + SQL Server.
- Unit tests for the service layer.
