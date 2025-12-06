using AutoMapper;
using BaseChord.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using BaseChord.Application.Validation;
using FluentValidation;
using MediatR;

namespace BaseChord.Application;

/// <summary>
/// Provides extension methods for configuring application-specific services, such as AutoMapper and MediatR.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Configures and registers application-specific services, such as AutoMapper for mapping configurations
    /// and MediatR for handling application requests and behaviors, into the provided IServiceCollection.
    /// </summary>
    /// <param name="services">
    /// The collection of service descriptors to which the application services will be added.
    /// </param>
    /// <returns>
    /// The updated IServiceCollection with the added application-specific services configured.
    /// </returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddApplicationAutoMapper();
        services.AddApplicationMediatR();

        return services;
    }

    /// <summary>
    /// Extends the AutoMapper configuration to include dynamic mappings by discovering all types
    /// implementing the IMap<,> interface and registering their mappings at runtime.
    /// </summary>
    /// <param name="cfg">
    /// The AutoMapper configuration expression used to define mapping rules between types.
    /// </param>
    /// <param name="assemblies">
    /// An array of assemblies to scan for types implementing the IMap interface. Currently unused as the method
    /// scans all assemblies in the current application domain.
    /// </param>
    public static void AddIMapConfiguration(this IMapperConfigurationExpression cfg, params Assembly[] assemblies)
    {
        var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        appDomainAssemblies.AddRange(assemblies);
        var mappings = appDomainAssemblies.DistinctBy(x => x.FullName).SelectMany(x => x.GetTypes()).Where(x => x.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMap<,>)));

        MethodInfo createMapMethodInfo = typeof(IProfileExpression)
            .GetMethods().Single(x => x.Name == nameof(IProfileExpression.CreateMap) && x.GetGenericArguments().Length == 2 && !x.GetParameters().Any());
        foreach (Type mapping in mappings)
        {
            object? map = Activator.CreateInstance(mapping);

            foreach (Type interfaceType in mapping.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMap<,>)))
            {
                Type sourceType = interfaceType.GetGenericArguments().ElementAt(0);
                Type destinationType = interfaceType.GetGenericArguments().ElementAt(1);

                MethodInfo specificCreateMapMethodInfo = createMapMethodInfo.MakeGenericMethod(sourceType, destinationType);
                MethodInfo specificMapMethod = typeof(IMap<,>).MakeGenericType(sourceType, destinationType).GetMethods().Single();
                object mappingExpression = specificCreateMapMethodInfo.Invoke(cfg, [])!;

                specificMapMethod.Invoke(map, [
                    mappingExpression
                ]);
            }
        }
    }

    private static void AddApplicationMediatR(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    }

    private static void AddApplicationAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddIMapConfiguration());
    }
}
