using ECommerce.Core.Entities.Category;
using ECommerce.Core.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageManagementService _imageManagementService;
    private readonly IValidator<AddCategoryDTO> _addCategoryDtoValidator;
    private readonly IValidator<UpdateCategoryDTO> _updateCategoryDtoValidator;

    public CategoryService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        IImageManagementService imageManagementService,
        IValidator<AddCategoryDTO> addCategoryDtoValidator,
        IValidator<UpdateCategoryDTO> updateCategoryDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageManagementService = imageManagementService;
        _addCategoryDtoValidator = addCategoryDtoValidator;
        _updateCategoryDtoValidator = updateCategoryDtoValidator;
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
        var validationResult = await _addCategoryDtoValidator.ValidateAsync(categoryDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        var category = _mapper.Map<Category>(categoryDTO);
        await _unitOfWork.CategoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCategoryAsync(UpdateCategoryDTO categoryDTO, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateCategoryDtoValidator.ValidateAsync(categoryDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
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

        var folderPath = $"categories/{id}";
        await _imageManagementService.DeleteFolderAsync(folderPath, cancellationToken);
        Category categoryStub = new Category { Id = id };
        await _unitOfWork.CategoryRepository.DeleteAsync(categoryStub, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}