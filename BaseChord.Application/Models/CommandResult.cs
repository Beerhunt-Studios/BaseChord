using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Models;

/// <summary>
/// This class contains the result of an command
/// </summary>
/// <typeparam name="T">Type of the result</typeparam>
public class CommandResult<T>
{
    /// <summary>
    /// The result
    /// </summary>
    public T? Result { get; set; }

    /// <summary>
    /// The type of result
    /// </summary>
    public CommandResultTypeEnum Type { get; set; }

    /// <summary>
    /// Constructor of a CommandResult
    /// </summary>
    /// <param name="result">The result which is <c>Nullable</c></param>
    /// <param name="type">The type of result <see cref="CommandResultTypeEnum"/></param>
    public CommandResult(T? result, CommandResultTypeEnum type)
    {
        Result = result;
        Type = type;
    }
}
