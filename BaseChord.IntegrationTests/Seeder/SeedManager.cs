using BaseChord.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ThreadSafe;

namespace BaseChord.IntegrationTests.Seeder;

public class SeedManager
{
    private readonly ThreadSafeDbContext _dbContext;
    private readonly IDbTransactionFactory _dbTransactionFactory;
    private readonly ISeeder[] _seeders;

    public SeedManager(ThreadSafeDbContext dbContext, IDbTransactionFactory dbTransactionFactory, IEnumerable<ISeeder> seeders)
    {
        _dbContext = dbContext;
        _dbTransactionFactory = dbTransactionFactory;
        _seeders = seeders.ToArray();
    }

    public async Task SeedData()
    {
        IDbTransaction dbTransaction = await _dbTransactionFactory.CreateTransaction();
        foreach (ISeeder seeder in _seeders)
        {
            _dbContext.AddRange(seeder.Seed());
        }
        
        await dbTransaction.CommitAsync();
    }
}