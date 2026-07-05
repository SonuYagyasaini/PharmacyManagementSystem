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

**Local .NET & EF tool setup (recommended)**

- **Install .NET 8 SDK (user-scoped, no admin):**

```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --install-dir $HOME/.dotnet --architecture arm64
```

- **Install `dotnet-ef` tool (user-scoped):**

```bash
$HOME/.dotnet/dotnet tool install --global dotnet-ef --version 8.*
# ensure $HOME/.dotnet/tools is on your PATH (add to ~/.zprofile or ~/.zshrc)
```

- **Run EF commands using the local install (avoids system-level dotnet):**

```bash
# temporarily use the local dotnet and tools for the command
PATH=$HOME/.dotnet:$HOME/.dotnet/tools DOTNET_ROOT=$HOME/.dotnet /Users/$(whoami)/.dotnet/tools/dotnet-ef migrations add InitialCreate --project PharmacyManagement.Api.csproj --startup-project PharmacyManagement.Api.csproj
PATH=$HOME/.dotnet:$HOME/.dotnet/tools DOTNET_ROOT=$HOME/.dotnet /Users/$(whoami)/.dotnet/tools/dotnet-ef database update --project PharmacyManagement.Api.csproj --startup-project PharmacyManagement.Api.csproj
```

- **Run the API using the local dotnet:**

```bash
PATH=$HOME/.dotnet:$HOME/.dotnet/tools DOTNET_ROOT=$HOME/.dotnet $HOME/.dotnet/dotnet run --project PharmacyManagement.Api.csproj
```

Add the PATH lines to `~/.zprofile` or `~/.zshrc` to make them permanent:

```bash
echo 'export PATH="$PATH:$HOME/.dotnet:$HOME/.dotnet/tools"' >> ~/.zprofile
```
