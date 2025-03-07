using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Application.Database;

public interface IDbTransaction
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    void Dispose();
}
