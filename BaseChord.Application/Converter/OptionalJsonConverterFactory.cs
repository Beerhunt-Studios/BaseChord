using System.Text.Json;
using System.Text.Json.Serialization;
using BaseChord.Application.Models;

namespace BaseChord.Application.Converter;

/// <summary>
/// Factory for creating JSON converters for Optional&lt;T&gt; types.
/// </summary>
public class OptionalJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc cref="JsonConverterFactory.CanConvert"/>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    /// <inheritdoc cref="JsonConverterFactory.CreateConverter"/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var innerType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionalJsonConverter<>).MakeGenericType(innerType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}