using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Api.Infrastructure.Auditing;
using PharmacyManagement.Api.Infrastructure.Exceptions;
using PharmacyManagement.Api.Infrastructure.Logging;
using PharmacyManagement.Api.Infrastructure.Persistence;
using PharmacyManagement.Api.Modules.Medicines;
using PharmacyManagement.Api.Modules.Sales;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://127.0.0.1:5170");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PharmacyDb")));

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ABC Pharmacy Management API",
        Version = "v1",
        Description = "Modular monolithic .NET 9 API with SQL Server, EF Core Code First, logging, and audit tracking."
    });
});

// Configure Minimal API JSON options to be case-insensitive and support DateOnly
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new PharmacyManagement.Api.Infrastructure.Json.DateOnlyJsonConverter());
});

builder.Services.AddMedicineModule();
builder.Services.AddSalesModule();
builder.Services.AddScoped<AuditLogService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ABC Pharmacy Management API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseRequestLogging();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseCors("FrontendPolicy");

app.MapGet("/", () => Results.Ok(new
{
    application = "ABC Pharmacy Management API",
    architecture = "Modular Monolith",
    database = "SQL Server with EF Core Code First",
    features = new[]
    {
        "Centralized exception handling",
        "SQL request/response logging",
        "SQL audit logging for create/update/delete",
        "Medicine management",
        "Sale records"
    }
}));

app.MapMedicineEndpoints();
app.MapSalesEndpoints();
app.MapLoggingEndpoints();

app.Run();
