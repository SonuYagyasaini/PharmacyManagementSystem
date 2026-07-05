using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PharmacyManagement.Api.Infrastructure.Json;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.IsNullOrEmpty(s))
                return default;

            // Try parse ISO date or date-only
            if (DateOnly.TryParseExact(s, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                return d;

            if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return d;

            // Fallback to parsing as DateTime then take date portion
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                return DateOnly.FromDateTime(dt);
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var days))
        {
            // Support for numeric DateOnly ticks (rare)
            return DateOnly.FromDayNumber(days);
        }

        throw new JsonException($"Unable to convert token of type {reader.TokenType} to DateOnly.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
