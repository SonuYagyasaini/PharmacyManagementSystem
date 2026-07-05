# ABC Pharmacy Management API

ASP.NET Core 8 modular monolith using Entity Framework Core Code First with SQL Server.

## Cross-Cutting Features

- Global exception handling in `GlobalExceptionHandlingMiddleware`
- Clean JSON error response with status code, message, and trace id
- SQL request/response logging in `RequestResponseLogs`
- SQL audit logging in `AuditLogs` for create, update, delete, and stock changes
- Service-level logging in Medicine and Sales workflows
- Logging levels controlled from `appsettings.json`
- Read logs with `GET /api/logs/requests` and `GET /api/logs/audit`
- Swagger UI available at `/swagger` in development

## Run

```powershell
dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Open Swagger:

```text
http://localhost:{port}/swagger
```

For SQL Server Express, update `appsettings.json`:

```json
"PharmacyDb": "Server=.\\SQLEXPRESS;Database=AbcPharmacyDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```
