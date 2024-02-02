using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sync.Configuration;

public class SnakeCaseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null) return;
        if (schema.Properties.Count == 0) return;

        var keys = schema.Properties.Keys;
        var newProperties = new Dictionary<string, OpenApiSchema>();
        foreach (var key in keys)
        {
            newProperties[key.ToSnakeCase()] = schema.Properties[key];
        }

        schema.Properties = newProperties;
    }
}