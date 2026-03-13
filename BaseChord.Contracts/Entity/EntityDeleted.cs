namespace BaseChord.Contracts.Entity;

/// <summary>
/// Represents an event signaling that an existing entity was deleted
/// </summary>
public interface EntityDeleted : BaseContract
{
    /// <summary>
    /// The corresponding entityid
    /// </summary>
    public int EntityId { get; set; }
}