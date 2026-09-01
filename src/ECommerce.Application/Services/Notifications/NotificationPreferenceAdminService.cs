using AutoMapper;
using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Application.Specifications.Notifications;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services.Notifications;

public class NotificationPreferenceAdminService : INotificationPreferenceAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationPreferenceAdminService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<NotificationPreferenceDTO>> GetAllAsync()
    {
        var preferences = await _unitOfWork.GetRepository<NotificationPreference, Guid>()
            .GetAllAsync(new NotificationPreferenceSpecification());

        return _mapper.Map<IReadOnlyList<NotificationPreferenceDTO>>(preferences);
    }

    public async Task<NotificationPreferenceDTO> GetByIdAsync(Guid id)
    {
        var preference = await GetPreferenceByIdAsync(id);
        return _mapper.Map<NotificationPreferenceDTO>(preference);
    }

    public async Task<NotificationPreferenceDTO> CreateAsync(CreateNotificationPreferenceDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var repository = _unitOfWork.GetRepository<NotificationPreference, Guid>();
        var existing = await repository.GetFirstOrDefaultAsync(new NotificationPreferenceSpecification(dto.Type, dto.Channel));

        if (existing is not null)
            throw new BadRequestException($"Notification preference for type '{dto.Type}' and channel '{dto.Channel}' already exists.");

        var preference = _mapper.Map<NotificationPreference>(dto);
        await repository.AddAsync(preference);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationPreferenceDTO>(preference);
    }

    public async Task<NotificationPreferenceDTO> UpdateAsync(Guid id, UpdateNotificationPreferenceDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var preference = await GetPreferenceByIdAsync(id);
        var repository = _unitOfWork.GetRepository<NotificationPreference, Guid>();
        var duplicate = await repository.GetFirstOrDefaultAsync(new NotificationPreferenceSpecification(dto.Type, dto.Channel));

        if (duplicate is not null && duplicate.Id != id)
            throw new BadRequestException($"Notification preference for type '{dto.Type}' and channel '{dto.Channel}' already exists.");

        preference.Type = dto.Type;
        preference.Channel = dto.Channel;
        preference.Title = dto.Title;
        preference.Description = dto.Description;
        preference.DefaultEnabled = dto.DefaultEnabled;
        preference.IsActive = dto.IsActive;

        repository.Update(preference);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationPreferenceDTO>(preference);
    }

    public async Task DeleteAsync(Guid id)
    {
        var preference = await GetPreferenceByIdAsync(id);

        var userPreferenceCount = await _unitOfWork.GetRepository<UserNotificationPreference, Guid>()
            .CountAsync(new UserNotificationPreferencesByPreferenceSpecification(id));

        if (userPreferenceCount > 0)
        {
            preference.IsActive = false;
            _unitOfWork.GetRepository<NotificationPreference, Guid>().Update(preference);
        }
        else
        {
            _unitOfWork.GetRepository<NotificationPreference, Guid>().Delete(preference);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<NotificationPreference> GetPreferenceByIdAsync(Guid id)
    {
        var preference = await _unitOfWork.GetRepository<NotificationPreference, Guid>()
            .GetFirstOrDefaultAsync(new NotificationPreferenceSpecification(id));

        if (preference is null)
            throw new NotFoundException($"Notification preference '{id}' was not found.");

        return preference;
    }
}