namespace PharmacyManagement.Api.Modules.Medicines;

public static class MedicineModule
{
    public static IServiceCollection AddMedicineModule(this IServiceCollection services)
    {
        services.AddScoped<IMedicineRepository, MedicineRepository>();
        services.AddScoped<MedicineService>();
        return services;
    }

    public static IEndpointRouteBuilder MapMedicineEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/medicines");

        group.MapGet("/", async (string? search, string? sortBy, string? sortDirection, MedicineService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMedicinesAsync(search, sortBy, sortDirection, cancellationToken)));

        group.MapGet("/{id:guid}", async (Guid id, MedicineService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetMedicineAsync(id, cancellationToken)));

        group.MapPost("/", async (CreateMedicineRequest request, MedicineService service, CancellationToken cancellationToken) =>
        {
            var medicine = await service.AddMedicineAsync(request, cancellationToken);
            return Results.Created($"/api/medicines/{medicine.Id}", medicine);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateMedicineRequest request, MedicineService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.UpdateMedicineAsync(id, request, cancellationToken)));

        group.MapDelete("/{id:guid}", async (Guid id, MedicineService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteMedicineAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return app;
    }
}
