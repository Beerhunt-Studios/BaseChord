using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Exceptions;

/// <summary>
/// This exception describes what happens if an given entity was not found
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes the Exception with the given parameters
    /// </summary>
    /// <param name="entityType">The entitytype</param>
    /// <param name="id">The id</param>
    public NotFoundException(Type entityType, int id) : base(string.Format("The Entity \"{0}\" was not found while trying to find it with \"{1}\"", entityType.Name, id))
    {

    }
}
