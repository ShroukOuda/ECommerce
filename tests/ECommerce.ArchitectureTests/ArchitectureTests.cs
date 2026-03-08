using NetArchTest.Rules;
using FluentAssertions;

namespace ECommerce.ArchitectureTests;

public class ArchitectureTests
{
    private const string CoreNamespace = "ECommerce.Core";
    private const string ApplicationNamespace = "ECommerce.Application";
    private const string InfrastructureNamespace = "ECommerce.Infrastructure";
    private const string ApiNamespace = "ECommerce.API";

    [Fact]
    public void Core_ShouldNotDependOn_Application()
    {
        var result = Types.InAssembly(typeof(ECommerce.Core.Common.BaseEntity<>).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Core_ShouldNotDependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(ECommerce.Core.Common.BaseEntity<>).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Core_ShouldNotDependOn_Api()
    {
        var result = Types.InAssembly(typeof(ECommerce.Core.Common.BaseEntity<>).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(ECommerce.Application.ApplicationRegistration).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOn_Api()
    {
        var result = Types.InAssembly(typeof(ECommerce.Application.ApplicationRegistration).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_ShouldInheritFromBaseController()
    {
        var result = Types.InAssembly(typeof(ECommerce.API.Program).Assembly)
            .That()
            .ResideInNamespace("ECommerce.API.Controllers")
            .And()
            .AreNotAbstract()
            .And()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotHaveNameStartingWith("Base")
            .Should()
            .Inherit(typeof(ECommerce.API.Controllers.BaseController))
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Services_ShouldHaveNameEndingWithService()
    {
        var result = Types.InAssembly(typeof(ECommerce.Application.ApplicationRegistration).Assembly)
            .That()
            .ResideInNamespace("ECommerce.Application.Services")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Service")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Interfaces_InCore_ShouldStartWithI()
    {
        var result = Types.InAssembly(typeof(ECommerce.Core.Common.BaseEntity<>).Assembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entities_ShouldNotDependOn_Application()
    {
        var result = Types.InAssembly(typeof(ECommerce.Core.Common.BaseEntity<>).Assembly)
            .That()
            .ResideInNamespace("ECommerce.Core.Entities")
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
