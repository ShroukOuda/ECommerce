using AutoMapper;
using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Services.Notifications;
using ECommerce.Application.Specifications.Notifications;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Enums.Notification;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Specifications.Base;
using FluentAssertions;
using Moq;

namespace ECommerce.UnitTests.Services;

public class NotificationPreferenceServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<NotificationPreference, Guid>> _preferenceRepositoryMock;
    private readonly Mock<IGenericRepository<UserNotificationPreference, Guid>> _userPreferenceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserNotificationPreferenceService _service;

    public NotificationPreferenceServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _preferenceRepositoryMock = new Mock<IGenericRepository<NotificationPreference, Guid>>();
        _userPreferenceRepositoryMock = new Mock<IGenericRepository<UserNotificationPreference, Guid>>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock
            .Setup(u => u.GetRepository<NotificationPreference, Guid>())
            .Returns(_preferenceRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.GetRepository<UserNotificationPreference, Guid>())
            .Returns(_userPreferenceRepositoryMock.Object);

        _mapperMock
            .Setup(m => m.Map<UserNotificationPreferenceDTO>(It.IsAny<UserNotificationPreference>()))
            .Returns((UserNotificationPreference source) => new UserNotificationPreferenceDTO
            {
                Id = source.NotificationPreference.Id,
                Type = source.NotificationPreference.Type,
                Channel = source.NotificationPreference.Channel,
                Title = source.NotificationPreference.Title,
                Description = source.NotificationPreference.Description,
                IsEnabled = source.IsEnabled
            });

        _service = new UserNotificationPreferenceService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetPreferencesAsync_ShouldReturnActivePreferencesWithUserOverrides()
    {
        var activePreferences = new List<NotificationPreference>
        {
            new()
            {
                Id = TestGuid.FromInt(1),
                Type = NotificationType.NewProduct,
                Channel = NotificationChannel.Email,
                Title = "New products",
                Description = "Receive new product updates",
                DefaultEnabled = true,
                IsActive = true
            },
            new()
            {
                Id = TestGuid.FromInt(2),
                Type = NotificationType.Promotion,
                Channel = NotificationChannel.InApp,
                Title = "Promotions",
                Description = "Receive promotional messages",
                DefaultEnabled = false,
                IsActive = true
            }
        };

        var userPreferences = new List<UserNotificationPreference>
        {
            new()
            {
                NotificationPreferenceId = TestGuid.FromInt(1),
                IsEnabled = false,
                NotificationPreference = activePreferences[0]
            }
        };

        _preferenceRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<BaseSpecification<NotificationPreference, Guid>>()))
            .ReturnsAsync(activePreferences);

        _userPreferenceRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<BaseSpecification<UserNotificationPreference, Guid>>()))
            .ReturnsAsync(userPreferences);

        var result = await _service.GetPreferencesAsync("user-1");

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(TestGuid.FromInt(1));
        result[0].IsEnabled.Should().BeFalse();
        result[1].Id.Should().Be(TestGuid.FromInt(2));
        result[1].IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task UpdatePreferenceAsync_WhenRowExists_ShouldUpdateOnlyEnabledState()
    {
        var preference = new NotificationPreference
        {
            Id = TestGuid.FromInt(3),
            Type = NotificationType.OrderPlaced,
            Channel = NotificationChannel.InApp,
            Title = "Order placed",
            Description = "Order updates",
            DefaultEnabled = true,
            IsActive = true
        };

        var existing = new UserNotificationPreference
        {
            Id = TestGuid.FromInt(4),
            UserId = "user-1",
            NotificationPreferenceId = preference.Id,
            IsEnabled = true,
            NotificationPreference = preference
        };

        _preferenceRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<NotificationPreference, Guid>>()))
            .ReturnsAsync(preference);

        _userPreferenceRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<UserNotificationPreference, Guid>>()))
            .ReturnsAsync(existing);

        await _service.UpdatePreferenceAsync("user-1", preference.Id, new UpdateUserNotificationPreferenceDTO
        {
            IsEnabled = false
        });

        _userPreferenceRepositoryMock.Verify(r => r.Update(It.Is<UserNotificationPreference>(x => x.Id == existing.Id && !x.IsEnabled)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePreferenceAsync_WhenRowMissing_ShouldCreateUserPreferenceRow()
    {
        var preference = new NotificationPreference
        {
            Id = TestGuid.FromInt(5),
            Type = NotificationType.SecurityAlert,
            Channel = NotificationChannel.InApp,
            Title = "Security alert",
            Description = "Security updates",
            DefaultEnabled = true,
            IsActive = true
        };

        _preferenceRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<NotificationPreference, Guid>>()))
            .ReturnsAsync(preference);

        _userPreferenceRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<UserNotificationPreference, Guid>>()))
            .ReturnsAsync((UserNotificationPreference?)null);

        await _service.UpdatePreferenceAsync("user-1", preference.Id, new UpdateUserNotificationPreferenceDTO
        {
            IsEnabled = false
        });

        _userPreferenceRepositoryMock.Verify(r => r.AddAsync(It.Is<UserNotificationPreference>(x =>
            x.UserId == "user-1" &&
            x.NotificationPreferenceId == preference.Id &&
            !x.IsEnabled)), Times.Once);
    }

    [Fact]
    public async Task TurnOffAllAsync_ShouldDisableExistingUserRowsOnly()
    {
        var preference = new NotificationPreference
        {
            Id = TestGuid.FromInt(6),
            Type = NotificationType.BackInStock,
            Channel = NotificationChannel.Email,
            Title = "Back in stock",
            Description = "Back in stock updates",
            DefaultEnabled = true,
            IsActive = true
        };

        var existing = new UserNotificationPreference
        {
            Id = TestGuid.FromInt(7),
            UserId = "user-1",
            NotificationPreferenceId = preference.Id,
            IsEnabled = true,
            NotificationPreference = preference
        };

        _userPreferenceRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<BaseSpecification<UserNotificationPreference, Guid>>()))
            .ReturnsAsync(new List<UserNotificationPreference> { existing });

        await _service.TurnOffAllAsync("user-1");

        _userPreferenceRepositoryMock.Verify(r => r.Update(It.Is<UserNotificationPreference>(x => !x.IsEnabled)), Times.Once);
    }

    [Fact]
    public async Task IsEnabledAsync_WhenUserHasNoRow_ShouldReturnDefaultEnabledValue()
    {
        var preference = new NotificationPreference
        {
            Id = TestGuid.FromInt(8),
            Type = NotificationType.Promotion,
            Channel = NotificationChannel.InApp,
            Title = "Promotion",
            Description = "Promotional updates",
            DefaultEnabled = false,
            IsActive = true
        };

        _preferenceRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<NotificationPreference, Guid>>()))
            .ReturnsAsync(preference);

        _userPreferenceRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<UserNotificationPreference, Guid>>()))
            .ReturnsAsync((UserNotificationPreference?)null);

        var result = await _service.IsEnabledAsync("user-1", preference.Id);

        result.Should().BeFalse();
    }
}