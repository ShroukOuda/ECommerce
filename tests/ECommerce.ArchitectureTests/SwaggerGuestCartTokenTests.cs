using ECommerce.API.Controllers;
using ECommerce.API.Swagger;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ECommerce.ArchitectureTests;

public class SwaggerGuestCartTokenTests
{
    private static OpenApiOperation Apply(Type controller, string method)
    {
        var methodInfo = controller.GetMethod(method)!;
        var api = new ApiDescription { ActionDescriptor = new ControllerActionDescriptor
        {
            ControllerTypeInfo = controller.GetTypeInfo(), ActionName = method, MethodInfo = methodInfo
        }};
        var operation = new OpenApiOperation();
        new GuestCartTokenOperationFilter().Apply(operation,
            new OperationFilterContext(api, null!, new SchemaRepository(), methodInfo));
        return operation;
    }

    [Fact]
    public void Claim_RequiresJwtAndGuestInSameRequirement()
    {
        var operation = Apply(typeof(CartController), nameof(CartController.Claim));
        operation.Security.Should().ContainSingle().Which.Keys.Select(x => x.Reference.Id)
            .Should().BeEquivalentTo("Bearer", "GuestCartToken");
    }

    [Theory]
    [InlineData(nameof(CartController.AddItem))]
    [InlineData(nameof(CartController.GetMyCart))]
    [InlineData(nameof(CartController.UpdateItem))]
    [InlineData(nameof(CartController.RemoveItem))]
    [InlineData(nameof(CartController.ClearCart))]
    public void CartRequests_AllowJwtOrGuest(string action)
    {
        var operation = Apply(typeof(CartController), action);
        operation.Security.Should().HaveCount(2);
        operation.Security.SelectMany(x => x.Keys).Select(x => x.Reference.Id)
            .Should().BeEquivalentTo("Bearer", "GuestCartToken");
        operation.Security.Should().OnlyContain(x => x.Count == 1);
    }

    [Fact]
    public void Session_AllowsAnonymousWithoutGuestHeader()
    {
        var operation = Apply(typeof(CartController), nameof(CartController.CreateCart));
        operation.Security.Should().Contain(x => x.Count == 0);
        operation.Security.SelectMany(x => x.Keys).Should().NotContain(x => x.Reference.Id == "GuestCartToken");
    }

    [Fact]
    public void NonCartEndpoint_IsUnchanged()
    {
        var operation = Apply(typeof(PaymentsController), nameof(PaymentsController.CreateCheckoutSession));
        operation.Security.Should().BeEmpty();
    }
}
