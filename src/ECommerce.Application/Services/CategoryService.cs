namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _unitOfWork.CategoryRepository.GetAllAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task AddCategoryAsync(AddCategoryDTO categoryDTO, CancellationToken cancellationToken = default)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        await _unitOfWork.CategoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCategoryAsync(UpdateCategoryDTO categoryDTO, CancellationToken cancellationToken = default)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        await _unitOfWork.CategoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Category ID must be greater than zero.", nameof(id));
        
        bool exist = await _unitOfWork.CategoryRepository.ExistsAsync(c => c.Id == id, cancellationToken);
        if (!exist)
            throw new KeyNotFoundException($"Category with ID {id} not found.");

        Category categoryStub = new Category { Id = id };
        await _unitOfWork.CategoryRepository.DeleteAsync(categoryStub, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}