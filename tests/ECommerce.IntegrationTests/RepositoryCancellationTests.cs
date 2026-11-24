using ECommerce.Application.Specifications.Payments;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Infrastructure.Persistence.Context;
using ECommerce.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.IntegrationTests;

public class RepositoryCancellationTests
{
    [Theory]
    [InlineData("by-id")]
    [InlineData("first")]
    [InlineData("all")]
    [InlineData("exists")]
    [InlineData("add")]
    public async Task CancelledToken_StopsRepositoryOperation(string operation)
    {
        // InMemory only. No startup, schema creation, seed or external database.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("cancellation-" + Guid.NewGuid()).Options;
        await using var context = new AppDbContext(options);
        var repository = new GenericRepository<Payment, Guid>(context);
        var specification = new LatestPaymentByOrderSpecification(Guid.NewGuid());
        using var source = new CancellationTokenSource();
        source.Cancel();
        Func<Task> act = operation switch
        {
            "by-id" => async () => { await repository.GetByIdAsync(Guid.NewGuid(), source.Token); },
            "first" => async () => { await repository.GetFirstOrDefaultAsync(specification, source.Token); },
            "all" => async () => { await repository.GetAllAsync(specification, source.Token); },
            "exists" => async () => { await repository.ExistsAsync(specification, source.Token); },
            _ => () => repository.AddAsync(new Payment(), source.Token)
        };
        await act.Should().ThrowAsync<OperationCanceledException>();
        context.ChangeTracker.Entries().Should().BeEmpty();
    }
}
