using ECommerce.Application.DTO.Return;
using ECommerce.Domain.Entities.Returns;
using ECommerce.Domain.Enums.Return;

namespace ECommerce.Application.Mapping;

public class ReturnMapping : Profile
{
    public ReturnMapping()
    {
        CreateMap<ReturnRequest, ReturnResponseDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.ReturnItems.Count));

        CreateMap<ReturnRequest, ReturnDetailsResponseDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.ReturnItems))
            .ForMember(d => d.History, o => o.MapFrom(s => s.ReturnHistories));

        CreateMap<ReturnRequest, AdminReturnDetailsResponseDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.ReturnItems))
            .ForMember(d => d.History, o => o.MapFrom(s => s.ReturnHistories))
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))
            .ForMember(d => d.UserEmail, o => o.MapFrom(s => s.User.Email));

        CreateMap<ReturnItem, ReturnItemResponseDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<ReturnHistory, ReturnHistoryResponseDTO>()
            .ForMember(d => d.OldStatus, o => o.MapFrom(s => s.OldStatus.ToString()))
            .ForMember(d => d.NewStatus, o => o.MapFrom(s => s.NewStatus.ToString()));
            
        CreateMap<ReturnHistory, AdminReturnHistoryResponseDTO>()
            .ForMember(d => d.OldStatus, o => o.MapFrom(s => s.OldStatus.ToString()))
            .ForMember(d => d.NewStatus, o => o.MapFrom(s => s.NewStatus.ToString()))
            .ForMember(d => d.PerformedById, o => o.MapFrom(s => s.PerformedById))
            .ForMember(d => d.PerformedByName, o => o.MapFrom(s => s.PerformedBy.UserName));

        CreateMap<CreateReturnRequestDTO, ReturnRequest>();
    }
}
