using BaseChord.Application.Repositories;
using BaseChord.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ThreadSafe;

namespace BaseChord.Infrastructure.Database.Repositories;

/// <inheritdoc/>
public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class, IEntity
{
    private readonly ThreadSafeDbContext _dbContext;

    /// <summary>
    /// Provides access to the underlying database set for entities of type <typeparamref name="TEntity"/>.
    /// This property is protected and enables derived repository classes to query and manipulate
    /// the entities within the database while ensuring encapsulation of the database context.
    /// </summary>
    protected DbSet<TEntity> Entities => _dbContext.Set<TEntity>();

    /// <summary>
    /// Provides access to the underlying thread-safe database context used in the repository.
    /// This property is protected and intended for internal use by derived repository classes.
    /// It facilitates database operations by exposing the context while preventing external manipulation.
    /// </summary>
    protected ThreadSafeDbContext Context => _dbContext;

    /// <summary>
    /// Provides a base implementation of the <see cref="IGenericRepository{TEntity}"/> interface for
    /// managing entities of type <typeparamref name="TEntity"/> in an Entity Framework Core context.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity managed by the repository. Must implement the <see cref="IEntity"/> interface.
    /// </typeparam>
    protected GenericRepository(ThreadSafeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        return (await _dbContext.Set<TEntity>().AddAsync(entity)).Entity;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveAsync(TEntity entity)
    {
        if (entity is IPersistentEntity persistenEntity)
        {
            persistenEntity.SetAsDeleted();

            return UpdateAsync(entity);
        }

        _dbContext.Set<TEntity>().Remove(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> ExitstAsync(int id)
    {
        return Entities.AnyAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(int id)
    {
        TEntity entity = await _dbContext.Set<TEntity>().Where(x => x.Id == id).SingleAsync();

        await RemoveAsync(entity);
    }

    /// <inheritdoc/>
    public Task<TEntity?> FindByEntityAsync(int id)
    {
        return _dbContext.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id);
    }
}
