using ECommerce.Application.DTO.Order;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerce.API.Swagger;

/// <summary>
/// Shows only cartId in the Swagger example body for POST orders.
/// The optional address IDs stay supported by the API; they are just not in the default example.
/// </summary>
public sealed class CreateOrderRequestExampleFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(CreateOrderDTO))
            return;

        schema.Example = new OpenApiObject
        {
            ["cartId"] = new OpenApiString(Guid.Empty.ToString())
        };
    }
}
