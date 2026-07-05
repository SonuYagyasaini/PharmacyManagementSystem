using System.Text.Json;
using PharmacyManagement.Api.Modules.Medicines;
using PharmacyManagement.Api.Modules.Sales;
using PharmacyManagement.Api.Infrastructure.Json;
using Xunit;

namespace PharmacyManagement.Tests;

public class DtoBindingTests
{
    private readonly JsonSerializerOptions _options;

    public DtoBindingTests()
    {
        _options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
        _options.Converters.Add(new DateOnlyJsonConverter());
    }

    [Fact]
    public void CreateMedicineRequest_Deserializes_FromCamelCaseJson()
    {
        var json = "{\"fullName\":\"X\",\"notes\":null,\"expiryDate\":\"2027-01-01\",\"quantity\":1,\"price\":2.5,\"brand\":\"B\"}";
        var dto = JsonSerializer.Deserialize<CreateMedicineRequest>(json, _options);
        Assert.NotNull(dto);
        Assert.Equal("X", dto!.FullName);
        Assert.Equal(1, dto.Quantity);
    }

    [Fact]
    public void CreateSaleRequest_Deserializes_FromCamelCaseJson()
    {
        var json = "{\"medicineId\":\"00000000-0000-0000-0000-000000000000\",\"quantity\":2}";
        var dto = JsonSerializer.Deserialize<CreateSaleRequest>(json, _options);
        Assert.NotNull(dto);
        Assert.Equal(2, dto!.Quantity);
    }
}
