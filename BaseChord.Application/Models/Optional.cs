using System.Text.Json.Serialization;
using BaseChord.Application.Converter;

namespace BaseChord.Application.Models;

/// <summary>
/// Represents an optional value that can distinguish between
/// "not provided", "explicitly null", and "actual value".
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[JsonConverter(typeof(OptionalJsonConverterFactory))]
public struct Optional<T>
{
    /// <summary>
    /// Indicates whether a value was provided in the request.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// The actual value provided, if <see cref="HasValue"/> is true.
    /// Can be null if explicitly set to null.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Creates a new Optional with a specified value.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    public Optional(T? value)
    {
        Value = value;
        HasValue = true;
    }

    /// <summary>
    /// Implicit conversion from T to Optional&lt;T&gt;.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Optional<T>(T value)
    {
        return new Optional<T>(value);
    }
}