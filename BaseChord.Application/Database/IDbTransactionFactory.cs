using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Database;

/// <summary>
/// Factory for creating database transactions
/// </summary>
public interface IDbTransactionFactory
{
    /// <summary>
    /// Creates a new database transaction
    /// </summary>
    /// <returns>A DatabaseTransaction</returns>
    public Task<IDbTransaction> CreateTransaction();
}
