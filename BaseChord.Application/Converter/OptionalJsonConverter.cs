using System.Text.Json;
using System.Text.Json.Serialization;
using BaseChord.Application.Models;

namespace BaseChord.Application.Converter;

/// <summary>
/// JSON converter for Optional&lt;T&gt; types.
/// Allows distinguishing between omitted values and explicit nulls.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
{
    /// <inheritdoc cref="JsonConverter{T}.Read"/>
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return new Optional<T>(value!);
    }

    /// <inheritdoc cref="JsonConverter{T}.Write"/>
    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}