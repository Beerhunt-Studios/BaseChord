using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Models;

/// <summary>
/// Describes the type of results an <see cref="CommandResult{T}"/> is able to respond
/// </summary>
public enum CommandResultTypeEnum
{
    /// <summary>
    /// The execution of the command was successfull
    /// </summary>
    Success,
    /// <summary>
    /// The given input for the command was invalid and couldn't be processed
    /// </summary>
    InvalidInput,
    /// <summary>
    /// There are conflicts in the backend, which indicates that something doesn't line up
    /// </summary>
    Conflict,
    /// <summary>
    /// One or more entities specified in the command weren't found
    /// </summary>
    NotFound
}
