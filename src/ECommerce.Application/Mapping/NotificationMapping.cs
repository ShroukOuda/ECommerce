using ECommerce.Domain.Entities.Notifications;
using ECommerce.Application.DTO.Notification;

namespace ECommerce.Application.Mapping;
public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDTO>();
        CreateMap<NotificationPreference, NotificationPreferenceDTO>();

        CreateMap<UserNotificationPreference, UserNotificationPreferenceDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NotificationPreference.Id))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.NotificationPreference.Type))
            .ForMember(dest => dest.Channel, opt => opt.MapFrom(src => src.NotificationPreference.Channel))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.NotificationPreference.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.NotificationPreference.Description));

        CreateMap<NotificationDTO, Notification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link));

        CreateMap<CreateNotificationDTO, Notification>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link));

        CreateMap<CreateNotificationPreferenceDTO, NotificationPreference>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<UpdateNotificationPreferenceDTO, NotificationPreference>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserPreferences, opt => opt.Ignore());

        CreateMap<UpdateUserNotificationPreferenceDTO, UserNotificationPreference>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.NotificationPreferenceId, opt => opt.Ignore())
            .ForMember(dest => dest.NotificationPreference, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}

