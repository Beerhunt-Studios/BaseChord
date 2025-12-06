using Microsoft.Extensions.Logging;

namespace BaseChord.Infrastructure.Database;

/// <summary>
/// Provides a mechanism for thread-safe locking to ensure proper synchronization of database transactions.
/// </summary>
/// <remarks>
/// This class is designed to manage locks for database transactions, ensuring that only one thread can hold the lock
/// at a time within a specified timeout period. The lock is constructed using a semaphore and works in conjunction
/// with transaction-related classes such as DbTransaction and DbTransactionFactory.
/// </remarks>
public class DbTransactionLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ILogger<DbTransactionLock> _logger;
    private Guid? _currentSemaphoreOwner = null;

    /// <summary>
    /// Represents a thread-safe locking mechanism for synchronizing database transactions.
    /// </summary>
    /// <remarks>
    /// The <see cref="DbTransactionLock"/> class uses a semaphore to control access to a shared resource
    /// and ensures only one thread can hold the lock at a time. It is particularly useful for coordinating
    /// concurrent database operations and preventing conflicting transactions within a given timeout period.
    /// </remarks>
    public DbTransactionLock(ILogger<DbTransactionLock> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously acquires a semaphore lock for synchronizing database transactions.
    /// </summary>
    /// <remarks>
    /// The method attempts to acquire a semaphore lock within a series of intervals, progressively increasing the wait time between attempts.
    /// If the lock is acquired, a unique identifier (GUID) is assigned to the lock owner. In case of failure to acquire the lock, an
    /// <see cref="AggregateException"/> may be thrown.
    /// </remarks>
    /// <returns>
    /// A <see cref="Guid"/> representing the unique identifier of the lock owner.
    /// </returns>
    /// <exception cref="AggregateException">
    /// Thrown if an exception occurs while waiting for the semaphore lock.
    /// </exception>
    public async Task<Guid> LockAsync()
    {
        _logger.LogDebug($"Trying to acquire lock from {Thread.CurrentThread.ManagedThreadId}");

        var task = _semaphore.WaitAsync(10000);

        var count = 0;
        do
        {
            count++;

            switch (count)
            {
                case 1:
                    _logger.LogWarning($"Could not acquire lock for {Thread.CurrentThread.ManagedThreadId}, waiting for 100ms");

                    continue;
                case 5:
                    _logger.LogWarning($"Could not acquire lock for {Thread.CurrentThread.ManagedThreadId}, waiting for 500ms");

                    continue;
                case 10:
                    _logger.LogWarning($"Could not acquire lock for {Thread.CurrentThread.ManagedThreadId}, waiting for 1s");

                    continue;

                case 25:
                    _logger.LogWarning($"Could not acquire lock for {Thread.CurrentThread.ManagedThreadId}, waiting for 2.5s");

                    continue;
                
                case 50:
                    _logger.LogWarning($"Could not acquire lock for {Thread.CurrentThread.ManagedThreadId}, waiting for 5s");

                    continue;
                
                case 75:
                    _logger.LogWarning($"Could not acquire lock for {Thread.CurrentThread.ManagedThreadId}, waiting for 7.5s");

                    continue;
                
                case 99:
                    _logger.LogWarning($"Could not acquire lock for {Thread.CurrentThread.ManagedThreadId}, waiting for 9.9s");

                    continue;
            }

            await Task.Delay(100);
        } while (!task.IsCompleted);

        if (task.Exception != null)
        {
            throw task.Exception;
        }

        _currentSemaphoreOwner = Guid.NewGuid();

        _logger.LogDebug($"Acquired lock from {Thread.CurrentThread.ManagedThreadId} with owner: {_currentSemaphoreOwner}");

        return _currentSemaphoreOwner.Value;
    }

    /// <summary>
    /// Releases the lock held by the specified owner.
    /// </summary>
    /// <param name="ownerId">
    /// The unique identifier of the owner currently holding the lock. This identifier is used to ensure
    /// that only the owner of the lock can release it.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the specified <paramref name="ownerId"/> does not match the current lock owner.
    /// </exception>
    public void Release(Guid ownerId)
    {
        if (_currentSemaphoreOwner != ownerId)
        {
            throw new InvalidOperationException("Invalid owner, the current owner is: " + _currentSemaphoreOwner + "but the owner requested to release is: " + ownerId + "");
        }

        _logger.LogDebug($"Releasing lock from {Thread.CurrentThread.ManagedThreadId} with owner: {_currentSemaphoreOwner}");

        _currentSemaphoreOwner = null;
        _semaphore.Release();
    }
}