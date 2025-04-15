using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Models;


/// <summary>
/// Describes the type of results an <see cref="QueryResult{T}"/> is able to respond
/// </summary>
public enum QueryResultType
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
    /// There are unproccessable entities envolved in the process
    /// </summary>
    UnprocessableEntity,
    /// <summary>
    /// One or more entities specified in the command weren't found
    /// </summary>
    NotFound,
}
