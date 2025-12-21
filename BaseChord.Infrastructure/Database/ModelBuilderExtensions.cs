using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Infrastructure.Database;

/// <summary>
/// Provides extension methods for configuring the <see cref="ModelBuilder"/>
/// with common infrastructure-related setup in a database context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures the <see cref="ModelBuilder"/> to include infrastructure-related setup,
    /// such as applying configurations from the specified assemblies and adding transactional outbox entities.
    /// </summary>
    /// <param name="modelBuilder">
    /// The <see cref="ModelBuilder"/> being extended.
    /// </param>
    /// <param name="assemblies">
    /// An array of assemblies containing entity type configurations to be applied to the model.
    /// </param>
    public static void AddInfrastructure(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }

    /// <summary>
    /// Configures the <see cref="ModelBuilder"/> to include event bus infrastructure-related setup,
    /// such as adding entities required for a transactional outbox pattern.
    /// </summary>
    /// <param name="modelBuilder">
    /// The <see cref="ModelBuilder"/> being extended.
    /// </param>
    public static void AddEventbusInfrastructure(this ModelBuilder modelBuilder)
    {
        modelBuilder.AddTransactionalOutboxEntities();
    }
}
