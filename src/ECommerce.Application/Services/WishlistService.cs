using ECommerce.Application.DTO.Wishlist;
using ECommerce.Domain.Entities.Wishlists;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Wishlists;

namespace ECommerce.Application.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddWishlistDTO> _addValidator;

    public WishlistService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AddWishlistDTO> addValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addValidator = addValidator;
    }

    public async Task<IEnumerable<GetWishlistDTO>> GetWishlistByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var spec = new WishlistsByUserSpecification(userId);
        var items = await _unitOfWork.GetRepository<Wishlist, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetWishlistDTO>>(items);
    }

    public async Task<GetWishlistDTO> AddToWishlistAsync(AddWishlistDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        
        var spec = new WishlistSpecification(dto.ProductId, dto.UserId);
        bool exists = await _unitOfWork.GetRepository<Wishlist, Guid>().ExistsAsync(spec);
        if (exists) throw new InvalidOperationException("Product already in wishlist.");
        
        var wishlist = _mapper.Map<Wishlist>(dto);
        await _unitOfWork.GetRepository<Wishlist, Guid>().AddAsync(wishlist, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetWishlistDTO>(wishlist);
    }

    public async Task RemoveFromWishlistAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new WishlistSpecification(id);
        var exist = await _unitOfWork.GetRepository<Wishlist, Guid>().ExistsAsync(spec);
        
        if (!exist) throw new KeyNotFoundException($"Wishlist item with ID {id} not found.");

        var stub = new Wishlist { Id = id};
        _unitOfWork.GetRepository<Wishlist, Guid>().Delete(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
