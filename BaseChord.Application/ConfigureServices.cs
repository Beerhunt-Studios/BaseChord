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

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddApplicationAutoMapper();
        services.AddApplicationMediatR();

        return services;
    }

    private static void AddApplicationMediatR(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    }

    private static void AddApplicationAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            var mappings = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => x.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMap<,>)));

            MethodInfo createMapMethodInfo = typeof(IProfileExpression).GetMethods()
                .Where(x => x.Name == nameof(IProfileExpression.CreateMap) && x.GetGenericArguments().Length == 2 && !x.GetParameters().Any()).Single();
            foreach (Type mapping in mappings)
            {
                object? map = Activator.CreateInstance(mapping);

                foreach (Type interfaceType in mapping.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMap<,>)))
                {
                    Type sourceType = interfaceType.GetGenericArguments().ElementAt(0);
                    Type destinationType = interfaceType.GetGenericArguments().ElementAt(1);

                    MethodInfo specifiyCreateMapMethodInfo = createMapMethodInfo.MakeGenericMethod(sourceType, destinationType);
                    MethodInfo specifiyMapMethod = typeof(IMap<,>).MakeGenericType(sourceType, destinationType).GetMethods().Single();
                    object mappingExpression = specifiyCreateMapMethodInfo.Invoke(cfg, new object[0])!;

                    specifiyMapMethod.Invoke(map, new object[]
                    {
                            mappingExpression
                    });
                }
            }
        });
    }
}
