using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaseChord.Api.Converter;

/// <summary>
/// Converts DateOnly to a string
/// </summary>
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    /// <inheritdoc cref="JsonConverter{T}.Read"/>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.FromDateTime(reader.GetDateTime());
    }

    /// <inheritdoc cref="JsonConverter{T}.Write"/>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O");
        writer.WriteStringValue(isoDate);
    }
}