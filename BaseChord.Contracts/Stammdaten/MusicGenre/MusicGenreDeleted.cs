namespace BaseChord.Contracts.Stammdaten.MusicGenre;

/// <summary>
/// Represents an event triggered when a music genre is deleted.
/// </summary>
public interface MusicGenreDeleted : BaseContract
{
    /// <summary>
    /// Gets the unique identifier of the music genre that has been added.
    /// This identifier is used to distinguish the genre within the system.
    /// </summary>
    public int Id { get; }
}