using ECommerce.Application.DTO.Address;
using ECommerce.Domain.Entities.Users;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Addresses;

namespace ECommerce.Application.Services;

public class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddAddressDTO> _addValidator;
    private readonly IValidator<UpdateAddressDTO> _updateValidator;

    public AddressService(IUnitOfWork unitOfWork, IMapper mapper,
        IValidator<AddAddressDTO> addValidator, IValidator<UpdateAddressDTO> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(string userId)
    {
        var spec = new AddressesByUserSpecification(userId);
        var addresses = await _unitOfWork.GetRepository<Address, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetAddressDTO>>(addresses);
    }

    public async Task<GetAddressDTO> GetAddressByIdAsync(Guid id, string userId)
    {
        var address = await _unitOfWork.GetRepository<Address,Guid>().GetByIdAsync(id);
        if (address is null || address.UserId != userId) throw new KeyNotFoundException($"Address with ID {id} not found.");
        return _mapper.Map<GetAddressDTO>(address);
    }

    public async Task<GetAddressDTO> AddAddressAsync(AddAddressDTO dto, string userId)
    {
        dto.UserId = userId;
        var validationResult = await _addValidator.ValidateAsync(dto);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
        var address = _mapper.Map<Address>(dto);
        address.UserId = userId;
        await _unitOfWork.GetRepository<Address, Guid>().AddAsync(address);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetAddressDTO>(address);
    }

    public async Task<GetAddressDTO> UpdateAddressAsync(Guid id, UpdateAddressDTO dto, string userId)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

         var spec = new AddressSpecification(id);
        var address = await _unitOfWork.GetRepository<Address, Guid>().GetFirstOrDefaultAsync(spec);
        if (address is null || address.UserId != userId) throw new KeyNotFoundException($"Address with ID {id} not found.");
       
        var originalId = address.Id;
        var originalCreatedAt = address.CreatedAt;
        _mapper.Map(dto, address);
        address.Id = originalId;
        address.CreatedAt = originalCreatedAt;
        _unitOfWork.GetRepository<Address, Guid>().Update(address);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetAddressDTO>(address);
    }

    public async Task DeleteAddressAsync(Guid id, string userId)
    {
        var address = await _unitOfWork.GetRepository<Address, Guid>().GetByIdAsync(id);
        if (address is null || address.UserId != userId)
            throw new KeyNotFoundException($"Address with ID {id} not found.");
        _unitOfWork.GetRepository<Address, Guid>().Delete(address);
        await _unitOfWork.SaveChangesAsync();
    }
}
