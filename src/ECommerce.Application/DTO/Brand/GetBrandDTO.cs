namespace ECommerce.Application.DTO.Brand;

public class GetBrandDTO : BrandBaseDTO
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
