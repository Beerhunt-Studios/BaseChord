using BaseChord.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseChord.Infrastructure.Database;

public class DbTransactionFactory : IDbTransactionFactory
{
    private readonly DbContext _context;
    private readonly ILogger _logger;

    public DbTransactionFactory(ILogger<DbTransaction> logger, DbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public DbTransactionFactory(ILogger logger, DbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IDbTransaction CreateTransaction()
    {
        return new DbTransaction(_logger, _context);
    }
}
