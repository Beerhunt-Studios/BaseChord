namespace BaseChord.Application.Database;

/// <summary>
/// Represents a database transaction
/// </summary>
public interface IDbTransaction : IDisposable
{
    /// <summary>
    /// Commits the transaction
    /// </summary>
    /// <param name="cancellationToken">CancellationToken to cancel the commit</param>
    /// <returns><see cref="Task"/> which finishes, when the commit is done</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
