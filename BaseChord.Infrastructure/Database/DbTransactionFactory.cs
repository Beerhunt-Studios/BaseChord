using BaseChord.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ThreadSafe;
using Microsoft.Extensions.Logging;

namespace BaseChord.Infrastructure.Database;

/// <summary>
/// Provides functionality for creating and managing database transactions.
/// This class is responsible for coordinating with the ThreadSafeDbContext and
/// ensuring transaction locks are properly handled.
/// </summary>
public class DbTransactionFactory : IDbTransactionFactory
{
    private readonly ThreadSafeDbContext _dbContext;
    private readonly DbTransactionLock _dbTransactionLock;
    private readonly ILogger<DbTransaction> _logger;

    /// <summary>
    /// Factory class for creating and managing instances of database transactions.
    /// It integrates with <see cref="ThreadSafeDbContext"/> to provide thread-safe
    /// database operations and utilizes a locking mechanism to ensure transactional integrity.
    /// </summary>
    public DbTransactionFactory(ILogger<DbTransaction> logger, DbTransactionLock dbTransactionLock, ThreadSafeDbContext context)
    {
        _logger = logger;
        _dbTransactionLock = dbTransactionLock;
        _dbContext = context;
    }

    /// <summary>
    /// Asynchronously creates a new database transaction, ensuring the proper use
    /// of transaction locking and integration with the thread-safe database context.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IDbTransaction"/> representing the newly created database transaction.
    /// </returns>
    public async Task<IDbTransaction> CreateTransaction()
    {
        var ownerLock = await _dbTransactionLock.LockAsync();
        
        return new DbTransaction(_logger, _dbContext, ownerLock, _dbTransactionLock);
    }
}
