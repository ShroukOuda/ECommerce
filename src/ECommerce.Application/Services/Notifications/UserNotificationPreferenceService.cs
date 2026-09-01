using AutoMapper;
using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Application.Specifications.Notifications;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Enums.Notification;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services.Notifications;

public class UserNotificationPreferenceService : IUserNotificationPreferenceService, INotificationPreferenceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserNotificationPreferenceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<UserNotificationPreferenceDTO>> GetPreferencesAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var activePreferences = await _unitOfWork.GetRepository<NotificationPreference, Guid>()
            .GetAllAsync(new ActiveNotificationPreferencesSpecification());

        var userPreferences = await _unitOfWork.GetRepository<UserNotificationPreference, Guid>()
            .GetAllAsync(new UserNotificationPreferencesSpecification(userId));

        var userPreferenceLookup = userPreferences
            .GroupBy(x => x.NotificationPreferenceId)
            .ToDictionary(g => g.Key, g => g.First());

        var result = activePreferences
            .Select(preference =>
            {
                var isEnabled = userPreferenceLookup.TryGetValue(preference.Id, out var userPreference)
                    ? userPreference.IsEnabled
                    : preference.DefaultEnabled;

                return _mapper.Map<UserNotificationPreferenceDTO>(new UserNotificationPreference
                {
                    NotificationPreference = preference,
                    IsEnabled = isEnabled,
                    UserId = userId
                });
            })
            .ToList();

        return result;
    }

    public async Task<UserNotificationPreferenceDTO> UpdatePreferenceAsync(
        string userId,
        Guid preferenceId,
        UpdateUserNotificationPreferenceDTO dto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentNullException.ThrowIfNull(dto);

        var preference = await GetActivePreferenceAsync(preferenceId);

        var repository = _unitOfWork.GetRepository<UserNotificationPreference, Guid>();
        var specification = new UserNotificationPreferenceByUserAndPreferenceSpecification(userId, preferenceId);
        var userPreference = await repository.GetFirstOrDefaultAsync(specification);

        if (userPreference is null)
        {
            userPreference = new UserNotificationPreference
            {
                UserId = userId,
                NotificationPreferenceId = preferenceId,
                IsEnabled = dto.IsEnabled,
                NotificationPreference = preference
            };

            await repository.AddAsync(userPreference);
        }
        else
        {
            userPreference.IsEnabled = dto.IsEnabled;
            repository.Update(userPreference);
        }

        await _unitOfWork.SaveChangesAsync();

        userPreference.NotificationPreference = preference;
        return _mapper.Map<UserNotificationPreferenceDTO>(userPreference);
    }

    public async Task TurnOffAllAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var repository = _unitOfWork.GetRepository<UserNotificationPreference, Guid>();
        var userPreferences = await repository.GetAllAsync(new UserNotificationPreferencesSpecification(userId));

        foreach (var userPreference in userPreferences)
        {
            if (userPreference.IsEnabled)
            {
                userPreference.IsEnabled = false;
                repository.Update(userPreference);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsEnabledAsync(string userId, Guid preferenceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var preference = await GetActivePreferenceAsync(preferenceId);
        return await IsEnabledAsync(userId, preference);
    }

    public async Task<bool> IsEnabledAsync(string userId, NotificationType type, NotificationChannel channel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var preference = await _unitOfWork.GetRepository<NotificationPreference, Guid>()
            .GetFirstOrDefaultAsync(new NotificationPreferenceSpecification(type, channel));

        if (preference is null || !preference.IsActive)
            throw new NotFoundException($"Notification preference '{type}' for channel '{channel}' was not found.");

        return await IsEnabledAsync(userId, preference);
    }

    private async Task<bool> IsEnabledAsync(string userId, NotificationPreference preference)
    {
        var userPreference = await _unitOfWork.GetRepository<UserNotificationPreference, Guid>()
            .GetFirstOrDefaultAsync(new UserNotificationPreferenceByUserAndPreferenceSpecification(userId, preference.Id));

        return userPreference?.IsEnabled ?? preference.DefaultEnabled;
    }

    private async Task<NotificationPreference> GetActivePreferenceAsync(Guid preferenceId)
    {
        var preference = await _unitOfWork.GetRepository<NotificationPreference, Guid>()
            .GetFirstOrDefaultAsync(new NotificationPreferenceSpecification(preferenceId));

        if (preference is null || !preference.IsActive)
            throw new NotFoundException($"Notification preference '{preferenceId}' was not found.");

        return preference;
    }
}