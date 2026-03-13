namespace BaseChord.Contracts.User;

/// <summary>
/// Represents an event signaling that a new user was created
/// </summary>
public interface UserCreated : BaseContract
{
    /// <summary>
    /// The corresponding userid of the created user
    /// </summary>
    public int UserId { get; set; }
}