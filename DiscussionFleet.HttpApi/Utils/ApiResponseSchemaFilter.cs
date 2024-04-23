using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DiscussionFleet.HttpApi.Utils;

public class ApiResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(ApiResponse))
        {
            schema.Example = new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["code"] = new OpenApiInteger(0),
                ["message"] = new OpenApiString("string"),
                ["data"] = new OpenApiNull()
            };
        }
    }
}