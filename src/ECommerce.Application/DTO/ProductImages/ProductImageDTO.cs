using ECommerce.Application.DTO.Common;

namespace ECommerce.Application.DTO.ProductImages;

public class ProductImageDTO : BaseImageDTO
{
    public bool IsMain { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
}