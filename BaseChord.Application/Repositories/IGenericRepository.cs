using BaseChord.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Repositories;

/// <summary>
/// Represents a generic repository for performing CRUD operations on entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity that the repository will work with. The entity type must conform to the <see cref="IEntity"/> interface.
/// </typeparam>
public interface IGenericRepository<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Asynchronously adds an entity to the repository and returns the added entity.
    /// </summary>
    /// <param name="entity">
    /// The entity to be added. The entity must conform to the <see cref="IEntity"/> interface and must not be null.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the entity after it has been added to the repository.
    /// </returns>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Asynchronously updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to be updated. The entity must conform to the <see cref="IEntity"/> interface and must not be null.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Asynchronously removes an entity from the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to be removed. The entity must conform to the <see cref="IEntity"/> interface and must not be null.
    /// If the entity implements the <see cref="IPersistentEntity"/> interface, it will be marked as deleted instead of being fully removed.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task RemoveAsync(TEntity entity);

    /// <summary>
    /// Asynchronously checks if an entity with the specified identifier exists in the repository.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to be checked.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the entity exists in the repository.
    /// </returns>
    Task<bool> ExitstAsync(int id);

    /// <summary>
    /// Asynchronously removes an entity from the repository.
    /// </summary>
    /// <param name="id">
    /// The id of the entity to be removed.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task RemoveAsync(int id);

    /// <summary>
    /// Asynchronously finds an entity by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity to be retrieved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the entity if it exists in the repository, or null if no entity matches the provided identifier.
    /// </returns>
    Task<TEntity?> FindByEntityAsync(int id);
}
