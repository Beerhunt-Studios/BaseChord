namespace BaseChord.Contracts.Stammdaten.MusicGenre;

/// <summary>
/// Represents an event triggered when a music genre is deleted.
/// </summary>
public interface MusicGenreDeleted : BaseContract
{
    /// <summary>
    /// The unique identifier of the deleted music genre.
    /// </summary>
    public int Id { get; }
}