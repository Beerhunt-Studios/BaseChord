namespace BaseChord.Contracts.Stammdaten.MusicGenre;

/// <summary>
/// Represents an event signaling that a new music genre has been added.
/// </summary>
public interface MusicGenreAdded : BaseContract
{
    public int Id { get; }
    
    public int? ParentId { get; }
    
    public string Text { get; }
}