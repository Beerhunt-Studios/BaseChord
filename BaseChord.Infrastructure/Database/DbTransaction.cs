using BaseChord.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ThreadSafe;
using Microsoft.Extensions.Logging;

namespace BaseChord.Infrastructure.Database;

public class DbTransaction : IDbTransaction, IDisposable
{
    private readonly ThreadSafeDbContext _context;
    private readonly ILogger _logger;
    private IDbContextTransaction? _transaction;

    public DbTransaction(ILogger logger, ThreadSafeDbContext context)
    {
        _logger = logger;
        _context = context;

        _transaction = _context.Database.BeginTransaction();
        _logger.LogDebug("Transaction {0} started", _transaction.TransactionId);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Something went wrong, the underlying DatabaseTransaction is already closed!");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved changes to transaction {0}", _transaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "While saving the changes to the database, an exception occured!");

            throw;
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Commited Transaction {0}", _transaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit transaction {0}", _transaction.TransactionId);

            throw;
        }

        _transaction.Dispose();
        _transaction = null;
    }

    public void Dispose()
    {
        if (_transaction == null)
        {
            return;
        }

        _logger.LogWarning("Transaction not commited and it's gonna get reverted");
        _transaction.Rollback();
        _transaction.Dispose();

        _logger.LogInformation("Database Entries are reloaded to wipe the changes");
        _context.ChangeTracker.DetectChanges();
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            entry.Reload();
        }
    }
}
