namespace BaseChord.Contracts.Entity;

/// <summary>
/// Represents an event signaling that a new entity was created
/// </summary>
public interface EntityCreated : BaseContract
{
    /// <summary>
    /// The corresponding entityid of the created entity
    /// </summary>
    public int EntityId { get; set; }
}