namespace ECommerce.Application.DTO.Category;

public class GetCategoryDTO : CategoryBaseDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = null!;
    public List<string> CategoryImage { get; set; } = new();
   
}