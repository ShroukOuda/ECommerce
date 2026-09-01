namespace ECommerce.Application.DTO.Category;

public class GetCategoryDetailDTO : CategoryBaseDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = null!;
    public Guid? ParentCategoryId { get; set; }
    public GetCategoryDTO? ParentCategory { get; set; }
    public List<GetCategoryDTO> SubCategories { get; set; } = new();
    public List<string> CategoryImages { get; set; } = new();
}