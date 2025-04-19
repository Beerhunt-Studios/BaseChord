using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using BaseChord.Domain;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseChord.Infrastructure.Database
{
    /// <summary>
    /// Provides extension methods for <see cref="ModelBuilder"/> to configure the database schema.
    /// These methods include adding infrastructure configurations and applying mappings for <see cref="ValueObject"/> types.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Applies infrastructure-specific configurations from the specified assemblies and adds transactional outbox entities.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance to apply configurations to.</param>
        /// <param name="assemblies">The assemblies that contain the configuration classes.</param>
        public static void AddInfrastructure(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }

            modelBuilder.ApplyValueObjectMappings();
            modelBuilder.AddTransactionalOutboxEntities();
        }

        /// <summary>
        /// Automatically applies mappings for <see cref="ValueObject"/> properties in entities.
        /// This method configures each <see cref="ValueObject"/> as an owned type in the database using <see cref="OwnsOne"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance to apply the mappings to.</param>
        private static void ApplyValueObjectMappings(this ModelBuilder modelBuilder)
        {
            var entities = modelBuilder.Model.GetEntityTypes()
                .Where(e => e.ClrType?.IsClass == true);

            foreach (IMutableEntityType entity in entities)
            {
                Type entityClrType = entity.ClrType;

                // Get properties that are ValueObjects
                var valueObjectProperties = entityClrType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => typeof(ValueObject).IsAssignableFrom(p.PropertyType));

                foreach (PropertyInfo prop in valueObjectProperties)
                {
                    // Dynamically apply OwnsOne configuration to owned value object properties
                    var entityBuilderGeneric = typeof(ModelBuilder)
                        .GetMethod(nameof(ModelBuilder.Entity), 1, Type.EmptyTypes)?
                        .MakeGenericMethod(entityClrType)
                        .Invoke(modelBuilder, null);

                    MethodInfo? ownsOneMethod = entityBuilderGeneric?.GetType()
                        .GetMethods()
                        .FirstOrDefault(m => m.Name == nameof(EntityTypeBuilder<object>.OwnsOne)
                                             && m.GetParameters().Length == 2)?
                        .MakeGenericMethod(prop.PropertyType);

                    if (ownsOneMethod != null)
                    {
                        // Create a lambda expression to map the property
                        var parameter = Expression.Parameter(entityClrType, "e");
                        var property = Expression.Property(parameter, prop);
                        var lambda = Expression.Lambda(property, parameter);

                        // Apply the OwnsOne method dynamically
                        ownsOneMethod.Invoke(entityBuilderGeneric, new object[]
                        {
                            lambda, null!
                        });
                    }
                }
            }
        }
    }
}
