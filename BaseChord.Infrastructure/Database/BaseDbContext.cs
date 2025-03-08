using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BaseChord.Infrastructure.Database;

public abstract class BaseDbContext: DbContext
{
    protected abstract Assembly LoadingAssemlby { get; }

    public BaseDbContext() : base()
    {

    }

    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(this.LoadingAssemlby);

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}
