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

    public async Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var spec = new AddressesByUserSpecification(userId);
        var addresses = await _unitOfWork.GetRepository<Address, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetAddressDTO>>(addresses);
    }

    public async Task<GetAddressDTO> GetAddressByIdAsync(Guid id, CancellationToken ct = default)
    {
        var address = await _unitOfWork.GetRepository<Address,Guid>().GetByIdAsync(id);
        if (address is null) throw new KeyNotFoundException($"Address with ID {id} not found.");
        return _mapper.Map<GetAddressDTO>(address);
    }

    public async Task<GetAddressDTO> AddAddressAsync(AddAddressDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        var address = _mapper.Map<Address>(dto);
        await _unitOfWork.GetRepository<Address, Guid>().AddAsync(address, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetAddressDTO>(address);
    }

    public async Task<GetAddressDTO> UpdateAddressAsync(Guid id, UpdateAddressDTO dto, CancellationToken ct = default)
    {
        var result = await _updateValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

         var spec = new AddressSpecification(id);
        bool exists = await _unitOfWork.GetRepository<Address, Guid>().ExistsAsync(spec, ct);
        if (!exists) throw new KeyNotFoundException($"Address with ID {id} not found.");
       
        var address = _mapper.Map<Address>(dto);
        _unitOfWork.GetRepository<Address, Guid>().Update(address, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetAddressDTO>(address);
    }

    public async Task DeleteAddressAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new AddressSpecification(id);
        bool exists = await _unitOfWork.GetRepository<Address, Guid>().ExistsAsync(spec, ct);
        if (!exists) throw new KeyNotFoundException($"Address with ID {id} not found.");
        var stub = new Address { Id = id };
        _unitOfWork.GetRepository<Address, Guid>().Delete(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
