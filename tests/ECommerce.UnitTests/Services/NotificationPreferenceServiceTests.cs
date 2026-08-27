using AutoMapper;
using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Services.Notifications;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Enums.Notification;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace ECommerce.UnitTests.Services;

public class NotificationPreferenceServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<UserNotificationPreference, Guid>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly NotificationPreferenceService _service;

    public NotificationPreferenceServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IGenericRepository<UserNotificationPreference, Guid>>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock
            .Setup(u => u.GetRepository<UserNotificationPreference, Guid>())
            .Returns(_repositoryMock.Object);

        _service = new NotificationPreferenceService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetPreferencesAsync_ShouldReturnMappedPreferences()
    {
        var preferences = new List<UserNotificationPreference>
        {
            new()
            {
                Id = TestGuid.FromInt(1),
                UserId = "user-1",
                Type = NotificationType.NewProduct,
                Channel = NotificationChannel.Email,
                IsEnabled = true
            }
        };

        var dto = new List<UserNotificationPreferenceDTO>
        {
            new()
            {
                Id = preferences[0].Id,
                UserId = "user-1",
                Type = NotificationType.NewProduct,
                Channel = NotificationChannel.Email,
                IsEnabled = true
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(preferences);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<UserNotificationPreferenceDTO>>(preferences))
            .Returns(dto);

        var result = await _service.GetPreferencesAsync("user-1");

        result.Should().HaveCount(1);
        result[0].Type.Should().Be(NotificationType.NewProduct);
    }

    [Fact]
    public async Task UpdatePreferenceAsync_WhenPreferenceExists_ShouldUpdateIt()
    {
        var existing = new UserNotificationPreference
        {
            Id = TestGuid.FromInt(2),
            UserId = "user-1",
            Type = NotificationType.BackInStock,
            Channel = NotificationChannel.InApp,
            IsEnabled = true
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserNotificationPreference> { existing });

        await _service.UpdatePreferenceAsync("user-1", new UpdateNotificationPreferenceDTO
        {
            Type = NotificationType.BackInStock,
            Channel = NotificationChannel.InApp,
            IsEnabled = false
        });

        _repositoryMock.Verify(r => r.Update(It.Is<UserNotificationPreference>(x => x.Id == existing.Id && !x.IsEnabled), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveAllPreferencesAsync_ShouldPersistNewAndUpdatedPreferences()
    {
        var existing = new UserNotificationPreference
        {
            Id = TestGuid.FromInt(3),
            UserId = "user-1",
            Type = NotificationType.OrderPlaced,
            Channel = NotificationChannel.Email,
            IsEnabled = true
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserNotificationPreference> { existing });

        await _service.SaveAllPreferencesAsync("user-1", new SaveNotificationPreferencesDto
        {
            Preferences = new List<UpdateNotificationPreferenceDTO>
            {
                new() { Type = NotificationType.OrderPlaced, Channel = NotificationChannel.Email, IsEnabled = false },
                new() { Type = NotificationType.Promotion, Channel = NotificationChannel.InApp, IsEnabled = true }
            }
        });

        _repositoryMock.Verify(r => r.Update(It.Is<UserNotificationPreference>(x => x.Type == NotificationType.OrderPlaced && !x.IsEnabled), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<UserNotificationPreference>(x => x.Type == NotificationType.Promotion && x.UserId == "user-1"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IsEnabledAsync_WhenNoPreferenceExists_ShouldReturnTrue()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserNotificationPreference>());

        var result = await _service.IsEnabledAsync("user-1", NotificationType.SecurityAlert);

        result.Should().BeTrue();
    }
}
