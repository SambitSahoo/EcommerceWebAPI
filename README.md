
Project Overview
Brief description of your Ecommerce Web API project, its purpose, and technologies used (Docker, RabbitMQ, SQL Server, EF Core,Postgre Sql etc.).
Prerequisites
•	.NET 8 SDK
•	Docker & Docker Compose
•	VS Code with SQL Server extension
•	pgAdmin (for Discount API)

Database Scaffolding for your Local System
--------------------------------------------
dotnet ef dbcontext scaffold \
  "Server=localhost,1433;Database=sample;User Id=sa;Password=Your Password;TrustServerCertificate=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project MyWebApi/MyWebApi.csproj \
  --output-dir Models \
  --context MyDbContext \
  --table Categories \
  --table Products \
  --table OrderItems \
  --table Orders \
  --data-annotations \
  --force
  
Running Services with Docker
----------------------------
For Catalog API:
----------------
docker-compose -f docker-compose/docker-compose.yml -f docker-compose/docker-compose.override.yml up --build

Basket API
------------
docker-compose down
docker-compose up --build

Discount API (Postgre Admin)
------------------------
Steps to connect via pgAdmin with credentials:
1. Open your browser and go to: http://localhost:8005 -> you can see this Url in docker-compose.yml file also
2. Log in with:
    * Email: "Your Email"
    * Password: "Your Password"
3. Add a new server:
    * Name: discountdb
    * Host: discountdb (this is the container name)
    * Port: 5432
    * Username: postgres
    * Password: Your Password
4. Once connected:
    * Expand the server → Databases → DiscountDb
    * Navigate to Schemas → public → Tables
    * You should see all the tables created by your EF Core migration (e.g., Coupons, etc.)

Order API Setup
----------------
Scaffolding:

scaffolding: dotnet ef dbcontext scaffold \
"Server=localhost,1433;Database=OrderDb;User Id=sa;Password=Your Password;TrustServerCertificate=True" \
Microsoft.EntityFrameworkCore.SqlServer \
--output-dir Entities \
--context OrderDbContext \
--context-dir Context \
--use-database-names \
--force

How to Connect Sql Server in VS Code using docker image for OrderDb
—————————------------------------------------------------------------
1.Open VS Code->Click Sql Server -> Add Connection-> In the window 1.Profile name:- any name like Ecommerce

2. Servername: localhost,1433
3. Username: sa
4. Password: Your Password
5. Database name : type master
6. Click OK
7. Once connected create a new database named as OrderDb as per connection string name.
8. Then create a new table I.e PurchaseOrders here as per Table name  NOTE: when you run docker-compose down, it removes all containers, networks, and volumes defined in your docker-compose.yml.
9. That includes the volume where your SQL Server data (OrderDb) lives — unless you've explicitly named and persisted it.
10.  How to stop containers without deleting data —— Instead of using docker-compose down ,Use docker-compose stop
11. To start everything again: docker-compose start


If any changes you done in docker-compose.yml file then 
docker-compose up --build

EXAMPLE:-
=========
In our Application for Docker/VsCode/SqlServer
Profile name:- any name like Ecommerce
The Db name is  : OrderDb
Table Name is : PurchaseOrders  Db creation script: Create database OrderDb

Table Creation Script
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrders](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserName] [nvarchar](100) NULL,
    [TotalPrice] [decimal](18, 2) NULL,
    [FirstName] [nvarchar](50) NULL,
    [LastName] [nvarchar](50) NULL,
    [EmailAddress] [nvarchar](100) NULL,
    [AddressLine] [nvarchar](200) NULL,
    [Country] [nvarchar](50) NULL,
    [State] [nvarchar](50) NULL,
    [ZipCode] [nvarchar](20) NULL,
    [CardName] [nvarchar](50) NULL,
    [CardNumber] [nvarchar](20) NULL,
    [Expiration] [nvarchar](10) NULL,
    [CVV] [nvarchar](10) NULL,
    [PaymentMethod] [int] NULL,
    [LastModifiedDate] [datetime] NULL,
    [CreatedBy] [nvarchar](100) NULL,
    [LastModifiedBy] [nvarchar](100) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PurchaseOrders] ADD PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PurchaseOrders] ADD  DEFAULT (getdate()) FOR [LastModifiedDate]
GO
ALTER TABLE [dbo].[PurchaseOrders] ADD  DEFAULT ('system') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[PurchaseOrders] ADD  DEFAULT ('system') FOR [LastModifiedBy]
GO

How to see the logs in Elastic Search & Kibana
———————————------------------------------------
Go to http://localhost:5601/ it will open the elastic home page
Click Analytics
Then click Discover

If nothing will show, then top right corner change from last One hour to last 1 week or something like that
 Then logs will show. in the Filter your data using Kql search box put the below Kql query
 fields.ApplicationName: "Basket.API"

 --------------------
 Quick Endpoint Map (based on your setup)
 
API Service	   Host URL	                      Swagger UI	    Likely Base Route
Catalog API	   http://localhost:8000	        /swagger	      /api/catalog
Basket API	   http://localhost:8001	        /swagger	      /api/basket
Discount API	 http://localhost:8002	        /swagger	      /api/discount
Order API	     http://localhost:8004	        /swagger	      /api/order
