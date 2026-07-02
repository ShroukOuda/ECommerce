using ECommerce.Application.DTO.Inventory;
using ECommerce.Domain.Entities.Inventories;

namespace ECommerce.Application.Mapping;

public class InventoryMapping : Profile
{
    public InventoryMapping()
    {
        CreateMap<CreateInventoryHistoryDTO, InventoryHistory>()
            .ForMember(d => d.ChangeType, o => o.MapFrom(s => Enum.Parse<ECommerce.Domain.Enums.Inventory.InventoryChangeType>(s.ChangeType)));
        CreateMap<InventoryHistory, GetInventoryHistoryDTO>()
            .ForMember(d => d.ChangeType, o => o.MapFrom(s => s.ChangeType.ToString()));
    }
}
