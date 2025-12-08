namespace BaseChord.Contracts.Stammdaten.MusicGenre;

/// <summary>
/// Represents an event signaling that a new music genre has been added.
/// </summary>
public interface MusicGenreAdded : BaseContract
{
    /// <summary>
    /// Gets the unique identifier of the music genre that has been added.
    /// This identifier is used to distinguish the genre within the system.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the identifier of the parent genre, if available.
    /// This identifier establishes a hierarchical relationship between music genres,
    /// linking subgenres to their respective parent genres.
    /// </summary>
    public int? ParentId { get; }

    /// <summary>
    /// Gets the descriptive name or label of the music genre that has been added.
    /// This property provides a human-readable representation of the genre.
    /// </summary>
    public string Text { get; }
}