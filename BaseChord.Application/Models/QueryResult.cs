using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Models;

/// <summary>
/// This class contains the result of an query
/// </summary>
/// <typeparam name="T">Type of the result</typeparam>
public class QueryResult<T>
{
    /// <summary>
    /// The result
    /// </summary>
    public T? Result { get; set; }

    /// <summary>
    /// The type of result
    /// </summary>
    public QueryResultType Type { get; set; }

    /// <summary>
    /// Constructor of a QueryResult
    /// </summary>
    /// <param name="result">The result which is <c>Nullable</c></param>
    /// <param name="type">The type of result <see cref="QueryResultType"/></param>
    public QueryResult(T? result, QueryResultType type)
    {
        Result = result;
        Type = type;
    }
}
