using ECommerce.API.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerce.API.Swagger;

/// <summary>Documents the existing cart capability header without changing authorization.</summary>
public sealed class GuestCartTokenOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor action
            || action.ControllerTypeInfo.AsType() != typeof(CartController))
            return;

        OpenApiSecurityScheme Reference(string id) => new()
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = id }
        };

        var bearer = Reference("Bearer");
        var guest = Reference("GuestCartToken");
        operation.Security = action.ActionName switch
        {
            // Claim requires both the signed-in customer and the original guest capability.
            nameof(CartController.Claim) => new List<OpenApiSecurityRequirement>
            {
                new() { [bearer] = Array.Empty<string>(), [guest] = Array.Empty<string>() }
            },
            // Session can be anonymous, or attach to the signed-in account. No guest token needed.
            nameof(CartController.CreateCart) => new List<OpenApiSecurityRequirement>
            {
                new(), new() { [bearer] = Array.Empty<string>() }
            },
            _ => new List<OpenApiSecurityRequirement>
            {
                new() { [bearer] = Array.Empty<string>() },
                new() { [guest] = Array.Empty<string>() }
            }
        };
    }
}
