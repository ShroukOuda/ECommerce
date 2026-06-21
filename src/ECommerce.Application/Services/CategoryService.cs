using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Category;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly IValidator<AddCategoryDTO> _addCategoryDtoValidator;
    private readonly IValidator<UpdateCategoryDTO> _updateCategoryDtoValidator;

    public CategoryService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        IFileStorageService fileStorageService,
        IValidator<AddCategoryDTO> addCategoryDtoValidator,
        IValidator<UpdateCategoryDTO> updateCategoryDtoValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _addCategoryDtoValidator = addCategoryDtoValidator;
        _updateCategoryDtoValidator = updateCategoryDtoValidator;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _unitOfWork.CategoryRepository.GetAllAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        return category;
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

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        bool exist = await _unitOfWork.CategoryRepository.ExistsAsync(c => c.Id == id, cancellationToken);
        if (!exist)
            throw new KeyNotFoundException($"Category with ID {id} not found.");

        var folderPath = $"categories/{id}";
        await _fileStorageService.DeleteFolderAsync(folderPath, cancellationToken);
        Category categoryStub = new Category { Id = id };
        await _unitOfWork.CategoryRepository.DeleteAsync(categoryStub, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}