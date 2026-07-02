using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;
using ECommerce.Application.DTO.Product;

namespace ECommerce.Domain.Specifications.Products;

public class ProductSpecification : BaseSpecification<Product, Guid>
{
    public ProductSpecification(Product)
    {
        
    }
}