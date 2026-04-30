using ECommerce.Application.DTO.Wishlist;
using ECommerce.Domain.Entities.Wishlist;
using ECommerce.Domain.Interfaces.Repositories;

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
        var items = await _unitOfWork.WishlistRepository.GetWishlistByUserIdAsync(userId, ct);
        return _mapper.Map<IEnumerable<GetWishlistDTO>>(items);
    }

    public async Task AddToWishlistAsync(AddWishlistDTO dto, CancellationToken ct = default)
    {
        var result = await _addValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        
        bool exists = await _unitOfWork.WishlistRepository.ExistsAsync(
            w => w.ProductId == dto.ProductId && w.UserId == dto.UserId, ct);
        if (exists) throw new InvalidOperationException("Product already in wishlist.");
        
        var wishlist = _mapper.Map<Wishlist>(dto);
        await _unitOfWork.WishlistRepository.AddAsync(wishlist, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveFromWishlistAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _unitOfWork.WishlistRepository.GetByIdAsync(id, ct);
        if (item is null) throw new KeyNotFoundException($"Wishlist item with ID {id} not found.");
        await _unitOfWork.WishlistRepository.DeleteAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
