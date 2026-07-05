namespace PharmacyManagement.Api.Modules.Sales;

public static class SaleModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services)
    {
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<SaleService>();
        return services;
    }

    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sales");

        group.MapGet("/", async (SaleService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetSalesAsync(cancellationToken)));

        group.MapPost("/", async (CreateSaleRequest request, SaleService service, CancellationToken cancellationToken) =>
        {
            var sale = await service.CreateSaleAsync(request, cancellationToken);
            return Results.Created($"/api/sales/{sale.Id}", sale);
        });

        return app;
    }
}
