using AutoMapper;
using ECommerce.Application.Services.Notifications;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace ECommerce.UnitTests.Services;

public class NotificationServiceTests
{
    [Fact]
    public async Task MissingNotification_IsTypedNotFoundAndDoesNotSave()
    {
        var repository = new Mock<IGenericRepository<Notification, Guid>>();
        var unit = new Mock<IUnitOfWork>();
        unit.Setup(x => x.GetRepository<Notification, Guid>()).Returns(repository.Object);
        var service = new NotificationService(unit.Object, new Mock<IMapper>().Object);
        var act = () => service.MarkAsReadAsync("owner", Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
        unit.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task OtherOwnersNotification_IsIndistinguishableFromMissingAndUnchanged()
    {
        var id = Guid.NewGuid();
        var item = new Notification { Id = id, UserId = "other", IsRead = false };
        var repository = new Mock<IGenericRepository<Notification, Guid>>();
        repository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(item);
        var unit = new Mock<IUnitOfWork>();
        unit.Setup(x => x.GetRepository<Notification, Guid>()).Returns(repository.Object);
        var service = new NotificationService(unit.Object, new Mock<IMapper>().Object);
        var act = () => service.MarkAsReadAsync("owner", id);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Notification not found");
        item.IsRead.Should().BeFalse();
        unit.Verify(x => x.SaveChangesAsync(), Times.Never);
    }
}
