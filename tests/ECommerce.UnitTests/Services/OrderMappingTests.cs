using FluentAssertions;
using AutoMapper;
using ECommerce.Application.DTO.Order;
using ECommerce.Application.Mapping;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Enums.Payment;
using Microsoft.Extensions.Logging.Abstractions;

namespace ECommerce.UnitTests.Services;

public class OrderMappingTests
{
    private static IMapper Mapper() =>
        new MapperConfiguration(c => c.AddProfile<OrderMapping>(), NullLoggerFactory.Instance).CreateMapper();

    [Fact]
    public void PaymentStatus_WhenAnyPaymentPaid_ShouldBePaidEvenIfLaterAttemptFailed()
    {
        var order = new Order
        {
            Payments =
            {
                new Payment { Status = PaymentStatus.Paid, CreatedAt = new DateTime(2026, 1, 1) },
                new Payment { Status = PaymentStatus.Failed, CreatedAt = new DateTime(2026, 1, 2) }
            }
        };

        Mapper().Map<GetOrderDTO>(order).PaymentStatus.Should().Be("Paid");
        Mapper().Map<GetAdminOrderDTO>(order).PaymentStatus.Should().Be("Paid");
    }

    [Fact]
    public void PaymentStatus_WhenNonePaid_ShouldBeLatest()
    {
        var order = new Order
        {
            Payments =
            {
                new Payment { Status = PaymentStatus.Failed, CreatedAt = new DateTime(2026, 1, 1) },
                new Payment { Status = PaymentStatus.Pending, CreatedAt = new DateTime(2026, 1, 2) }
            }
        };

        Mapper().Map<GetOrderDTO>(order).PaymentStatus.Should().Be("Pending");
    }
}
