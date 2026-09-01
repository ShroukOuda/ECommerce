using ECommerce.Application.DTO.Inventory;
using ECommerce.Domain.Entities.Inventories;
using ECommerce.Domain.Enums.Inventory;

namespace ECommerce.Application.Mapping;

public class InventoryMapping : Profile
{
    public InventoryMapping()
    {
        CreateMap<CreateInventoryHistoryDTO, InventoryHistory>();
        
        CreateMap<InventoryHistory, GetInventoryHistoryDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
    }
}
