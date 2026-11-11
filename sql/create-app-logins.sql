-- Creates one least-privilege SQL Server login per service. Each login can only read/write data
-- in its own database: no DDL, no access to the other databases, no server-level roles.
--
-- Run as an administrator AFTER `dotnet ef database update` has created the databases.
-- Passwords come from sqlcmd scripting variables, which sqlcmd also reads from environment
-- variables of the same name, so they never have to be written into this file:
--
--   export CUSTOMER_APP_PASSWORD='...' ITEM_APP_PASSWORD='...' ORDER_APP_PASSWORD='...' GATEWAY_APP_PASSWORD='...'
--   sqlcmd -S localhost -U <admin-login> -C -i sql/create-app-logins.sql
--
-- Passwords must satisfy the server's password policy and must not contain a single quote.
-- The script is idempotent; existing logins are left unchanged.

:on error exit
SET NOCOUNT ON;
GO

USE [master];
IF SUSER_ID(N'customer_app') IS NULL
    CREATE LOGIN [customer_app] WITH PASSWORD = N'$(CUSTOMER_APP_PASSWORD)', CHECK_POLICY = ON, DEFAULT_DATABASE = [Customer];
IF SUSER_ID(N'item_app') IS NULL
    CREATE LOGIN [item_app] WITH PASSWORD = N'$(ITEM_APP_PASSWORD)', CHECK_POLICY = ON, DEFAULT_DATABASE = [Catalog];
IF SUSER_ID(N'order_app') IS NULL
    CREATE LOGIN [order_app] WITH PASSWORD = N'$(ORDER_APP_PASSWORD)', CHECK_POLICY = ON, DEFAULT_DATABASE = [Order];
IF SUSER_ID(N'gateway_app') IS NULL
    CREATE LOGIN [gateway_app] WITH PASSWORD = N'$(GATEWAY_APP_PASSWORD)', CHECK_POLICY = ON, DEFAULT_DATABASE = [Customer];
GO

-- CustomerGRPC: read/write. MicroSer gateway: read-only.
USE [Customer];
IF USER_ID(N'customer_app') IS NULL CREATE USER [customer_app] FOR LOGIN [customer_app];
ALTER ROLE db_datareader ADD MEMBER [customer_app];
ALTER ROLE db_datawriter ADD MEMBER [customer_app];
IF USER_ID(N'gateway_app') IS NULL CREATE USER [gateway_app] FOR LOGIN [gateway_app];
ALTER ROLE db_datareader ADD MEMBER [gateway_app];
GO

USE [Catalog];
IF USER_ID(N'item_app') IS NULL CREATE USER [item_app] FOR LOGIN [item_app];
ALTER ROLE db_datareader ADD MEMBER [item_app];
ALTER ROLE db_datawriter ADD MEMBER [item_app];
GO

USE [Order];
IF USER_ID(N'order_app') IS NULL CREATE USER [order_app] FOR LOGIN [order_app];
ALTER ROLE db_datareader ADD MEMBER [order_app];
ALTER ROLE db_datawriter ADD MEMBER [order_app];
GO
