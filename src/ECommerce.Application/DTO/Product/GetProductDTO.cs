namespace ECommerce.Application.DTO.Product;

public class GetProductDTO : ProductBaseDTO
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public List<PhotoDTO> Photos { get; set; }
}