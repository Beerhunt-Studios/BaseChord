namespace BaseChord.Contracts.User;

/// <summary>
/// Represents an event signaling that an existing user was deleted
/// </summary>
public interface UserRemoved : BaseContract
{
    /// <summary>
    /// The corresponding userid
    /// </summary>
    public int UserId { get; set; }
}