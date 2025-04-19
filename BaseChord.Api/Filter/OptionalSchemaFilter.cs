using BaseChord.Application.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BaseChord.Api.Filter;

/// <summary>
/// Unwraps Optional&lt;T&gt; for Swagger schema generation,
/// so that Swagger shows the actual property type.
/// </summary>
public class OptionalSchemaFilter : ISchemaFilter
{
    /// <inheritdoc cref="ISchemaFilter.Apply"/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>))
        {
            var innerType = type.GetGenericArguments()[0];
            var innerSchema = context.SchemaGenerator.GenerateSchema(innerType, context.SchemaRepository);

            schema.Type = innerSchema.Type;
            schema.Format = innerSchema.Format;
            schema.Properties = innerSchema.Properties;
            schema.Items = innerSchema.Items;
            schema.Nullable = true;
        }
    }
}