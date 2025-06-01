# Order Management System API  

## Overview  
This **.NET 8 Web API** manages orders efficiently, incorporating:  
- **Discount System:** Applies promotions dynamically based on customer segments.  
- **Order Status Tracking:** Implements clear state transitions (`Pending → Confirmed → Shipped → Delivered → Cancelled`).  
- **Order Analytics Endpoint:** Provides insights into **average order value** and **fulfillment time**.  

## Assumptions  
- **Discounting logic** dynamically applies rules (`GoldCustomerRule`, `LoyalCustomerRule`, etc.).  
- **JWT Authorization** secures API endpoints.  
- **Order status transitions** follow predefined **business rules**.  
- **Customer segmentation** is predefined, influencing promotions.    
- **Repository Pattern & Dependency Injection** ensure flexibility and maintainability.  
- **In-Memory DB for development** with the option to switch to **SQL Server** in `Program.cs`.  
- **Data is seeded automatically** when running the application for testing purposes.  

## Architecture & Technologies  
This project follows **Clean Architecture** with a **Layered Structure**:
- **API Layer:** Exposes endpoints using **ASP.NET Core MVC**.  
- **Application Layer:** Houses business logic and services (`OrderService`, `DiscountService`).  
- **Domain Layer:** Contains core entities and business rules (`OrderStatus`, `PromotionRule`).  
- **Infrastructure Layer:** Handles database interactions via **Entity Framework Core**.  
- **Tests Layer:** Implements **unit and integration tests** for robustness - test data also seeded.  

### Technologies Used  
#### **Core Framework & Language**
- **.NET 8** (`net8.0` as the target framework)
- **C#**

#### **API & Versioning**
- **Asp.Versioning.Mvc** (`8.1.0`) - API versioning for structured endpoint management
- **Asp.Versioning.Mvc.ApiExplorer** (`8.1.0`) - API documentation and discovery integration

#### **Database & ORM**
- **Entity Framework Core** (`8.0.16`) - ORM for database operations
- **Entity Framework Core Design** - Required for EF migrations and scaffolding
- **Entity Framework Core InMemory** - Used for unit testing and development mode
- **Entity Framework Core SqlServer** - SQL Server provider for production use
- **Entity Framework Core Tools** - Command-line support for migrations

#### **Testing & Development**
- **Microsoft.NET.Test.Sdk** (`17.14.0`) - Test framework for running unit and integration tests
- **xUnit** (`2.9.3`) - Testing framework for writing unit tests
- **xUnit.runner.visualstudio** (`3.1.0`) - Test runner for Visual Studio integration
- **Microsoft.AspNetCore.Mvc.Testing** (`8.0.16`) - Simplifies integration testing for ASP.NET Core applications

#### **API Documentation**
- **Swashbuckle.AspNetCore** (`6.6.2`) - Enables Swagger/OpenAPI documentation for the API
## Performance Considerations

The `OrderRepository` is built with several EF Core performance best practices:

- **Read-Only Queries with `AsNoTracking()`**  
  Improves performance by skipping EF Core’s change tracking for queries that don’t modify data.

- **Eager Loading with `Include()`**  
  Prevents multiple queries by loading related `Items` and `Customer` data in a single call.

- **Efficient Pagination with `Skip()` / `Take()`**  
  Reduces memory and database load when working with large datasets.

- **Consistent Ordering with `OrderByDescending()`**  
  Ensures stable pagination results by sorting orders by `CreatedAt`.

- **Optimized Lookups with `FindAsync()`**  
  Uses EF Core’s fast primary key lookup for retrieving individual records.

These optimizations help ensure scalability, responsiveness, and efficient data access throughout the system.

#### **Project References**
Solution includes multiple layered projects:
- **OrderManagementSystem.API** (API Layer)
- **OrderManagementSystem.Application** (Business Logic Layer)
- **OrderManagementSystem.Domain** (Core Entities & Business Rules)
- **OrderManagementSystem.Infrastructure** (Database & Repositories)

## Running the API  
### **Database Configuration**
By default, the API **uses an in-memory database**, but you can **switch to SQL Server** in `Program.cs`:

#### Using **In-Memory Database** (Default for Development)
csharp
builder.Services.AddDbContext<OrderManagementSystemContext>(options =>
    options.UseInMemoryDatabase("OrderManagementInMemoryDb"));
