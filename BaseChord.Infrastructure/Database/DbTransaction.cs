using BaseChord.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ThreadSafe;
using Microsoft.Extensions.Logging;

namespace BaseChord.Infrastructure.Database;

/// <summary>
/// Represents a database transaction that ensures consistency and atomicity of database operations.
/// Provides functionality to commit or roll back a transaction, and ensures safe resource disposal.
/// </summary>
public class DbTransaction : IDbTransaction, IDisposable
{
    private IDbContextTransaction? _transaction;
    private readonly DbContext _context;
    private readonly ILogger<DbTransaction> _logger;
    private readonly DbTransactionLock _dbTransactionLock;
    private readonly Guid _transactionLockOwnerId;

    /// <summary>
    /// Represents a database transaction that guarantees atomicity and consistency
    /// of database operations within a specific transactional scope.
    /// </summary>
    public DbTransaction(ILogger<DbTransaction> logger, ThreadSafeDbContext context, Guid transactionLockOwnerId, DbTransactionLock dbTransactionLock)
    {
        _logger = logger;
        _context = context;
        _transactionLockOwnerId = transactionLockOwnerId;
        _dbTransactionLock = dbTransactionLock;

        _transaction = _context.Database.BeginTransaction();
        _logger.LogDebug("Transaction {0} started", _transaction.TransactionId);
    }

    /// <summary>
    /// Asynchronously commits the current database transaction, persisting all changes made within the transaction scope.
    /// Ensures proper exception handling and resource cleanup during the commit process.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the commit operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous commit operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the underlying database transaction is already closed or unavailable.
    /// </exception>
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Something went wrong, the underlying DatabaseTransaction is already closed!");
        }

        try
        {
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
        }
        finally
        {
            _dbTransactionLock.Release(_transactionLockOwnerId);

            _transaction.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Releases all resources used by the current database transaction. Rolls back
    /// any changes not committed and restores the state of tracked entities in the context to
    /// their original values.
    /// </summary>
    /// <remarks>
    /// If the transaction was not committed, the method ensures that all changes are reverted by:
    /// - Rolling back the transaction.
    /// - Disposing of the underlying transaction instance.
    /// - Reloading entity states to synchronize with the current state in the database.
    /// Additionally, this method releases any locks associated with the transaction to
    /// allow other processes or threads to proceed.
    /// </remarks>
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
        
        _dbTransactionLock.Release(_transactionLockOwnerId);
    }
}