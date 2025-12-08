using MediatR;

namespace BaseChord.Contracts;

/// <summary>
/// Represents the foundational contract interface for all derived contracts.
/// Ensures the inclusion of a unique identifier property to standardize data tracking.
/// </summary>
public interface BaseContract : INotification
{
    /// <summary>
    /// Gets the unique identifier associated with the contract,
    /// used to standardize data tracking across implementations.
    /// </summary>
    public string SentryId { get; }
}