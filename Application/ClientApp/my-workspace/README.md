# JS-Entity-Framework (JRIAppAngular)

An enterprise-grade, client-side Entity Framework that brings the power of a complete `DbContext` to the browser, backed by a robust .NET RIA Data Service backend. 

Inspired by the structured data-management patterns of Silverlight RIA Services, this framework bridges the gap between complex HTML5 client-side applications (like Angular) and .NET Core backend ecosystems.

This implementation uses **Microsoft SQL Server** running the standard **AdventureWorksLT (Lightweight)** sample database.

## Key Features

* **Client-Side DbContext:** Full support for tracking entities, data modifications, and operational states natively in the browser.
* **Unit of Work Pattern:** Modify multiple records across detached data models or distinct forms, then **reject or submit updates in a single transaction**.
* **Two-Way Data Binding:** Seamlessly synchronize HTML form inputs with Entity properties using custom Angular directives.
* **Component UI Ready:** Validated to support data editing and change tracking on advanced table components like **PrimeNG DataTables**.

## Repository Architecture Breakdown

The project follows a clean **Decoupled Architecture** split across distinct layers:

### 📱 Frontend Client (TypeScript / Angular)
* **`Application/`**: Contains the Angular modules, viewmodels, and data-bound templates using the client framework. Includes components integrated with PrimeNG tables.

### ⚙️ Core Data & Transport Service (.NET Core / C#)
* **`RIAPP.DataService/`**: The fundamental core backend package implementing the RIA-based data service. It marshals tracking changes, handles client change-sets, and serializes transactions.
* **`RIAPP.DataService.EFCore/`**: The specialized extension package providing a robust translation bridge between the RIA transport layer and Entity Framework Core.

### 🏢 Demo Enterprise Application Layers (C#)
* **`RIAppDemo.DAL/`**: Data Access Layer containing the EF Core context maps, database model scaffoldings, and configurations for the **AdventureWorksLT** database.
* **`RIAppDemo.BLL/`**: Business Logic Layer containing data services, domain logic domain operations, custom validation rules, and the actual query processing endpoints.
* **`Extensions.Logging.RollingFile/`**: Dedicated diagnostics module enabling safe server-side file rolling logs to trace incoming transaction sets.

## Prerequisites

Before running the project, ensure you have the following installed:
* [Node.js](https://nodejs.org) (LTS version)
* [.NET 8.0 SDK](https://microsoft.com) or later
* [Microsoft SQL Server](https://microsoft.com) (Express, LocalDB, or Standard instance)
* [SQL Server Management Studio (SSMS)](https://microsoft.com)

## Getting Started

### 1. Database Setup
1. Download the `AdventureWorksLT.bak` backup file from the [Official Microsoft SQL Server Samples](https://microsoft.com).
2. Restore the database file to your target SQL Server Instance.
3. Update your server-side database connection string inside the `RIAppDemo.BLL` or `Application` settings (`appsettings.json`):

```json
{
  "ConnectionStrings": {
    "AdventureWorksLTConnection": "Server=YOUR_SERVER_NAME;Database=AdventureWorksLT;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Install Client Dependencies
Navigate to the root directory of your repository and restore npm dependencies:
```bash
npm install
```

### 3. Build the Framework Library
Compile the underlying `jriapp-lib` library via the Angular CLI before running the main app:
```bash
ng build jriapp-lib
```

### 4. Run the Application
Start compiling the main demo layout. The watch flag ensures hot-reloading during frontend development:
```bash
ng build --watch
```

## Architectural Design Pattern

```text
[ Angular UI ] <== (Two-Way Binding) ==> [ Client DbContext / Entities ]
||
(JSON Change-Set)
||
/
[ EF Core DbContext ] <=============== [ RIAPP.DataService.EFCore Layer ]
||
/
[ SQL Server (AdventureWorksLT) ]
```


### 1. Backend Entity Modeling (EF Core)
Your `RIAppDemo.DAL` tracks tables like `Customer`, `Product`, and `SalesOrderHeader` using normal DB mapping:

```csharp
// Example context binding inside RIAppDemo.DAL
public class AdventureWorksContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
}
```

### 2. Frontend Consumption (Angular)
The JRIApp DB component consumes the server endpoint exposed by the RIA service layer to populate properties seamlessly. Data elements utilize specialized properties to mirror state tracking instantly:

```html
<!-- Example of structural data binding mapping on client fields -->
<input [myBindDirective]="customer.FirstName" />
<button (click)="dbContext.submitChanges()">Submit Single Transaction</button>
```

## License

This project is licensed under the [MIT License](LICENSE).