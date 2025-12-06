using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Domain;

/// <summary>
/// Represents an entity that supports soft deletion within the domain model.
/// Implements additional functionality on top of the <see cref="IEntity"/> interface,
/// allowing entities to be marked as deleted without being permanently removed from storage.
/// </summary>
public interface IPersistentEntity : IEntity
{
    /// <summary>
    /// Gets a value indicating whether the entity has been marked as deleted.
    /// This property is typically used to implement soft deletion, allowing the entity
    /// to remain in storage but excluded from active query results.
    /// </summary>
    public bool IsDeleted { get; }

    /// <summary>
    /// Marks the entity as deleted by setting its "IsDeleted" property to true.
    /// This method facilitates the implementation of a soft delete pattern,
    /// where an entity is flagged as deleted rather than being permanently removed from the database or storage.
    /// </summary>
    public void SetAsDeleted();
}
