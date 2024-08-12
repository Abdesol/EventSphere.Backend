using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EventSphere.Api.Swagger.Filters;

/// <summary>
/// Schema filter to display DateOnly as a date in the swagger documentation.
/// </summary>
public class DateOnlySchemaFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(DateOnly))
        {
            schema.Properties.Clear();
            schema.Type = "string";
            schema.Format = "date";
        }
    }
}