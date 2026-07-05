# ABC Pharmacy Management API Documentation

## Project Goal

Build a pharmacy management Web API for ABC Pharmacy using ASP.NET Core 8. The API supports medicine management, sale records, SQL Server persistence, request/response logging, audit logging, exception handling, and Swagger API testing.

## Implementation Plan

1. Create a .NET 8 Web API project.
2. Follow a modular monolithic architecture.
3. Use Entity Framework Core Code First with SQL Server.
4. Create independent feature modules for medicines and sales.
5. Add centralized exception handling.
6. Add SQL-backed request/response logging.
7. Add SQL-backed audit logging for create, update, delete, and stock changes.
8. Add Swagger UI for testing APIs.
9. Seed initial medicine data through EF Core configuration.
10. Generate EF Core migration for database creation.

## Architecture Approach

The project uses a **Modular Monolithic Architecture**.

This means the application is deployed as one API, but the code is separated into clear modules. Each module owns its entity, DTOs, repository, service, and endpoints.

This approach is suitable because:

- The assessment is small to medium in scope.
- It avoids microservice complexity.
- It keeps features separated and easy to maintain.
- It supports future growth into separate services if needed.
- It works well with EF Core Code First and SQL Server.

## Project Structure

```text
PharmacyManagement.Api
  Infrastructure
    Auditing
      AuditAction.cs
      AuditLog.cs
      AuditLogConfiguration.cs
      AuditLogService.cs

    Exceptions
      ApiErrorResponse.cs
      BadRequestException.cs
      GlobalExceptionHandlingMiddleware.cs
      NotFoundException.cs

    Logging
      LoggingEndpoints.cs
      RequestLoggingExtensions.cs
      RequestResponseLog.cs
      RequestResponseLogConfiguration.cs

    Persistence
      PharmacyDbContext.cs

  Modules
    Medicines
      Medicine.cs
      MedicineConfiguration.cs
      MedicineDtos.cs
      MedicineModule.cs
      MedicineRepository.cs
      MedicineService.cs

    Sales
      Sale.cs
      SaleConfiguration.cs
      SaleDtos.cs
      SaleModule.cs
      SaleRepository.cs
      SaleService.cs

  Migrations
  Program.cs
  appsettings.json
```

## Technology Stack

- ASP.NET Core Web API 8
- Entity Framework Core 8 Code First
- SQL Server
- Minimal APIs
- Swagger / Swashbuckle
- Built-in .NET logging

## Database Approach

The project uses **EF Core Code First**.

Entities are created in C# first, then EF Core migrations generate the SQL Server database schema.

Main database tables:

- `Medicines`
- `Sales`
- `AuditLogs`
- `RequestResponseLogs`

Connection string location:

```json
"ConnectionStrings": {
  "PharmacyDb": "Server=(localdb)\\MSSQLLocalDB;Database=AbcPharmacyDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

For SQL Server Express:

```json
"ConnectionStrings": {
  "PharmacyDb": "Server=.\\SQLEXPRESS;Database=AbcPharmacyDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

## Medicine Module

The Medicine module manages medicine data.

Medicine fields:

- `Id`
- `FullName`
- `Notes`
- `ExpiryDate`
- `Quantity`
- `Price`
- `Brand`
- `CreatedAtUtc`
- `UpdatedAtUtc`
- `IsDeleted`
- `DeletedAtUtc`

Business rules:

- Medicine name is required.
- Brand is required.
- Quantity cannot be negative.
- Price must be greater than zero.
- Medicines expiring in less than 30 days return `HighlightColor = red`.
- Medicines with quantity less than 10 return `HighlightColor = yellow`.
- Delete uses soft delete, so records remain in SQL Server.

Medicine APIs:

```http
GET /api/medicines
GET /api/medicines?search=para
GET /api/medicines/{id}
POST /api/medicines
PUT /api/medicines/{id}
DELETE /api/medicines/{id}
```

## Sales Module

The Sales module maintains medicine sale records.

Sale fields:

- `Id`
- `MedicineId`
- `MedicineName`
- `Quantity`
- `UnitPrice`
- `TotalAmount`
- `SoldAtUtc`

Business rules:

- Sale quantity must be greater than zero.
- Medicine must exist.
- Medicine must have enough stock.
- Creating a sale reduces medicine stock.
- Sale creation is recorded in `AuditLogs`.
- Medicine stock change is also recorded in `AuditLogs`.

Sales APIs:

```http
GET /api/sales
POST /api/sales
```

## Logging Approach

The project has two types of logging.

### Request/Response Logging

Every API request is logged into the `RequestResponseLogs` table.

Stored information:

- Trace id
- HTTP method
- Path
- Query string
- Request body
- Response body
- Status code
- Elapsed milliseconds
- IP address
- User agent
- Requested UTC time

API to view request logs:

```http
GET /api/logs/requests
```

### Audit Logging

Business changes are logged into the `AuditLogs` table.

Stored information:

- Entity name
- Entity id
- Action
- Old values
- New values
- Request path
- Trace id
- Occurred UTC time

Logged actions:

- Medicine created
- Medicine updated
- Medicine deleted
- Sale created
- Medicine stock updated after sale

API to view audit logs:

```http
GET /api/logs/audit
```

## Exception Handling Approach

The project uses centralized exception handling through `GlobalExceptionHandlingMiddleware`.

It converts exceptions into clean JSON responses.

Example:

```json
{
  "statusCode": 404,
  "message": "Medicine was not found.",
  "traceId": "..."
}
```

Handled exceptions:

- `BadRequestException` returns HTTP 400
- `NotFoundException` returns HTTP 404
- `DbUpdateException` returns HTTP 409
- Unknown exceptions return HTTP 500

## Swagger

Swagger is added for API testing.

Swagger URL:

```text
http://localhost:{port}/swagger
```

Swagger is enabled in development environment.

## Initial Data

Initial data is seeded in `MedicineConfiguration.cs`.

### Medicine 1

```text
Id: 2b7cd386-89dd-44b9-ac6e-e921a1ccb81d
Full Name: Paracetamol 500mg Tablet
Notes: Use after food. Keep away from children.
Expiry Date: 2026-07-25
Quantity: 50
Price: 20.50
Brand: ABC Pharma
```

This medicine will show red highlight because the expiry date is within 30 days from July 4, 2026.

### Medicine 2

```text
Id: fa245e88-3244-426f-a0fa-b4c5bd6cd59f
Full Name: Cough Syrup 100ml
Notes: Shake well before use.
Expiry Date: 2027-02-10
Quantity: 7
Price: 85.00
Brand: HealthCare
```

This medicine will show yellow highlight because quantity is less than 10.

## Run Steps

Build project:

```powershell
dotnet build
```

Apply database migration:

```powershell
dotnet ef database update
```

Run API:

```powershell
dotnet run
```

Open Swagger:

```text
http://localhost:{port}/swagger
```

## Final Summary

The final solution is a modular monolithic .NET 8 API using SQL Server and EF Core Code First. It includes medicine management, sale records, logging, audit history, exception handling, Swagger, and initial seed data.
