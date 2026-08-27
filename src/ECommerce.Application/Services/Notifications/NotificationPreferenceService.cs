using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Enums.Notification;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services.Notifications;

public class NotificationPreferenceService : INotificationPreferenceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationPreferenceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<UserNotificationPreferenceDTO>> GetPreferencesAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is required.", nameof(userId));

        var preferences = await _unitOfWork.GetRepository<UserNotificationPreference, Guid>()
            .GetAllAsync();

        var userPreferences = preferences
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Channel)
            .ToList();

        return _mapper.Map<IReadOnlyList<UserNotificationPreferenceDTO>>(userPreferences);
    }

    public async Task UpdatePreferenceAsync(string userId, UpdateNotificationPreferenceDTO dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is required.", nameof(userId));

        ArgumentNullException.ThrowIfNull(dto);

        var repository = _unitOfWork.GetRepository<UserNotificationPreference, Guid>();
        var preferences = await repository.GetAllAsync();
        var preference = preferences.FirstOrDefault(x =>
            x.UserId == userId &&
            x.Type == dto.Type &&
            x.Channel == dto.Channel);

        if (preference is null)
        {
            preference = new UserNotificationPreference
            {
                UserId = userId,
                Type = dto.Type,
                Channel = dto.Channel,
                IsEnabled = dto.IsEnabled,
                CreatedAt = DateTime.UtcNow
            };

            await repository.AddAsync(preference);
        }
        else
        {
            preference.IsEnabled = dto.IsEnabled;
            preference.Channel = dto.Channel;
            preference.Type = dto.Type;
            repository.Update(preference);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SaveAllPreferencesAsync(string userId, SaveNotificationPreferencesDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is required.", nameof(userId));

        ArgumentNullException.ThrowIfNull(dto);

        var repository = _unitOfWork.GetRepository<UserNotificationPreference, Guid>();
        var existingPreferences = (await repository.GetAllAsync())
            .Where(x => x.UserId == userId)
            .ToList();

        foreach (var item in dto.Preferences)
        {
            var existingPreference = existingPreferences.FirstOrDefault(x =>
                x.Type == item.Type && x.Channel == item.Channel);

            if (existingPreference is null)
            {
                var newPreference = new UserNotificationPreference
                {
                    UserId = userId,
                    Type = item.Type,
                    Channel = item.Channel,
                    IsEnabled = item.IsEnabled,
                    CreatedAt = DateTime.UtcNow
                };

                await repository.AddAsync(newPreference);
                continue;
            }

            existingPreference.IsEnabled = item.IsEnabled;
            existingPreference.Channel = item.Channel;
            existingPreference.Type = item.Type;
            repository.Update(existingPreference);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task TurnOffAllAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is required.", nameof(userId));

        var repository = _unitOfWork.GetRepository<UserNotificationPreference, Guid>();
        var preferences = (await repository.GetAllAsync())
            .Where(x => x.UserId == userId)
            .ToList();

        foreach (var preference in preferences)
        {
            if (preference.IsEnabled)
            {
                preference.IsEnabled = false;
                repository.Update(preference);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsEnabledAsync(string userId, NotificationType type)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is required.", nameof(userId));

        var preferences = (await _unitOfWork.GetRepository<UserNotificationPreference, Guid>().GetAllAsync())
            .Where(x => x.UserId == userId && x.Type == type)
            .ToList();

        if (preferences.Count == 0)
            return true;

        return preferences.Any(x => x.IsEnabled);
    }
}
