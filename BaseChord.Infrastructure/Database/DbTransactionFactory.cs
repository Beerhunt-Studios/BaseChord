using BaseChord.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ThreadSafe;
using Microsoft.Extensions.Logging;

namespace BaseChord.Infrastructure.Database;

public class DbTransactionFactory : IDbTransactionFactory
{
    private readonly ThreadSafeDbContext _context;
    private readonly ILogger _logger;

    public DbTransactionFactory(ILogger<DbTransaction> logger, ThreadSafeDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public DbTransactionFactory(ILogger logger, ThreadSafeDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IDbTransaction CreateTransaction()
    {
        return new DbTransaction(_logger, _context);
    }
}
