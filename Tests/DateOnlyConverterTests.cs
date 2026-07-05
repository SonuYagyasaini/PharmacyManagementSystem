using System.Text.Json;
using PharmacyManagement.Api.Infrastructure.Json;
using Xunit;

namespace PharmacyManagement.Tests;

public class DateOnlyConverterTests
{
    [Fact]
    public void SerializesAndDeserializes_DateOnly_Correctly()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new DateOnlyJsonConverter());

        var original = System.DateOnly.Parse("2026-12-31");
        var json = JsonSerializer.Serialize(original, options);
        var round = JsonSerializer.Deserialize<System.DateOnly>(json, options);

        Assert.Equal(original, round);
    }
}
