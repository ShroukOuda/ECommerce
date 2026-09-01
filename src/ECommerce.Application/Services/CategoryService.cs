using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Categories;

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

    public async Task<IEnumerable<GetCategoryDTO>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.GetRepository<Category, Guid>().GetAllAsync();
        return _mapper.Map<IEnumerable<GetCategoryDTO>>(categories);
    }

    public async Task<GetCategoryDetailDTO> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.GetRepository<Category, Guid>().GetByIdAsync(id, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        return _mapper.Map<GetCategoryDetailDTO>(category);
    }

    public async Task<GetCategoryDTO> AddCategoryAsync(AddCategoryDTO categoryDTO, CancellationToken cancellationToken = default)
    {
        var validationResult = await _addCategoryDtoValidator.ValidateAsync(categoryDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        var category = _mapper.Map<Category>(categoryDTO);
        await _unitOfWork.GetRepository<Category, Guid>().AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<GetCategoryDTO>(category);
    }

    public async Task<GetCategoryDTO> UpdateCategoryAsync(Guid id, UpdateCategoryDTO categoryDTO, CancellationToken cancellationToken = default)
    {
        
        var validationResult = await _updateCategoryDtoValidator.ValidateAsync(categoryDTO);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var spec = new CategorySpecification(id);
        bool exist = await _unitOfWork.GetRepository<Category, Guid>().ExistsAsync(spec);
        if (!exist)
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        var category = _mapper.Map<Category>(categoryDTO);
        _unitOfWork.GetRepository<Category, Guid>().Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<GetCategoryDTO>(category);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spec = new CategorySpecification(id);
        bool exist = await _unitOfWork.GetRepository<Category, Guid>().ExistsAsync(spec);
        if (!exist)
            throw new KeyNotFoundException($"Category with ID {id} not found.");

        var folderPath = $"categories/{id}";
        await _fileStorageService.DeleteFolderAsync(folderPath, cancellationToken);
        Category categoryStub = new Category { Id = id };
        _unitOfWork.GetRepository<Category, Guid>().Delete(categoryStub, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}