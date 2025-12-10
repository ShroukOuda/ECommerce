namespace E_Commerece.Application.Services;

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

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _unitOfWork.CategoryRepository.GetByIdAsync(id);
    }

    public async Task AddCategoryAsync(AddCategoryDTO categoryDTO)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        await _unitOfWork.CategoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(UpdateCategoryDTO categoryDTO)
    {
        var category = _mapper.Map<Category>(categoryDTO);
        await _unitOfWork.CategoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _unitOfWork.CategoryRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}