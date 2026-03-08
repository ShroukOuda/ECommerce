using ECommerce.Application.DTO.Inventory;
using ECommerce.Core.Entities.Inventory;

namespace ECommerce.Application.Mapping;

public class InventoryMapping : Profile
{
    public InventoryMapping()
    {
        CreateMap<CreateInventoryHistoryDTO, InventoryHistory>()
            .ForMember(d => d.ChangeType, o => o.MapFrom(s => Enum.Parse<ECommerce.Core.Enums.Inventory.InventoryChangeType>(s.ChangeType)));
        CreateMap<InventoryHistory, GetInventoryHistoryDTO>()
            .ForMember(d => d.ChangeType, o => o.MapFrom(s => s.ChangeType.ToString()));
    }
}
