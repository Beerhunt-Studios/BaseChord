using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Infrastructure.Database;

public static class ModelBuilderExtensions
{
    public static void AddInfrastructure(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
