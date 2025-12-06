using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Domain;

/// <summary>
/// Represents a base interface for all entities in the domain model.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    public int Id { get; }
}
