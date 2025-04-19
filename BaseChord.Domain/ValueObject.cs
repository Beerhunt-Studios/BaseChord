namespace BaseChord.Domain;

/// <summary>
/// Represents a base class for value objects, implementing equality based on atomic values.
/// Inherits from <see cref="IEquatable{T}"/> and provides structural equality comparison.
/// Compatible with Entity Framework Core's owned types pattern.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Provides the atomic values of the value object that are used for equality comparison.
    /// Derived classes must override this method and yield return each component that should be used in equality and hash code calculations.
    /// </summary>
    /// <returns>An enumerable of atomic values used for equality.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current value object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        return Equals(obj as ValueObject);
    }

    /// <summary>
    /// Determines whether the specified value object is equal to the current value object.
    /// </summary>
    /// <param name="other">The value object to compare with the current value object.</param>
    /// <returns><c>true</c> if the specified value object is equal to the current value object; otherwise, <c>false</c>.</returns>
    public bool Equals(ValueObject? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns a hash code for the value object, based on its equality components.
    /// </summary>
    /// <returns>A hash code for the current value object.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (hash, obj) => HashCode.Combine(hash, obj));
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns><c>true</c> if the value objects are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        Equals(left, right);

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns><c>true</c> if the value objects are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !Equals(left, right);
}