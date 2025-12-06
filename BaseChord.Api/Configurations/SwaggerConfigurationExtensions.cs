using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BaseChord.Api.Filter;
using BaseChord.Application.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BaseChord.Api.Configurations;

internal static class SwaggerConfigurationExtensions
{
    internal static void UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

    }

    internal static void AddSwagger(this IServiceCollection services)
    {
        
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });

            // This coupled with the properties in the csproj allow the swagger page to show additional comments for methods
            var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
            
            options.SchemaFilter<OptionalSchemaFilter>();
            options.MapType(typeof(Optional<>), () => new OpenApiSchema { Nullable = true });
            
            options.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date",
                Example = new Microsoft.OpenApi.Any.OpenApiString("2025-04-19"),
            });

            options.MapType<DateOnly?>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date",
                Nullable = true,
                Example = new Microsoft.OpenApi.Any.OpenApiString("2025-04-19"),
            });
            
            options.RegisterKnownOptionalTypes();
        });
    }
    
    
    private static void RegisterKnownOptionalTypes(this SwaggerGenOptions options)
    {
        var knownTypes = new[]
        {
            typeof(string),
            typeof(int),
            typeof(int?),
            typeof(bool),
            typeof(bool?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateOnly),
            typeof(DateOnly?),
            typeof(Guid),
            typeof(Guid?)
        };

        foreach (var type in knownTypes)
        {
            var optionalType = typeof(Optional<>).MakeGenericType(type);

            var schema = new OpenApiSchema
            {
                Nullable = true,
            };

            if (type == typeof(string))
                schema.Type = "string";
            else if (type == typeof(int) || type == typeof(int?))
            {
                schema.Type = "integer";
                schema.Format = "int32";
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                schema.Type = "boolean";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                schema.Type = "string";
                schema.Format = "date-time";
            }
            else if (type == typeof(DateOnly) || type == typeof(DateOnly?))
            {
                schema.Type = "string";
                schema.Format = "date";
                schema.Example = new Microsoft.OpenApi.Any.OpenApiString("2025-04-19");
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                schema.Type = "string";
                schema.Format = "uuid";
            }

            options.MapType(optionalType, () => schema);
        }
    }
}
