using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;
using ECommerce.Application.DTO.Product;

namespace ECommerce.Application.Specifications.Products;

public class ProductCountSpecification : BaseSpecification<Product, Guid>
{

    public ProductCountSpecification()
    {
        
    }
    public ProductCountSpecification(ProductSpecParams productParams)
        : base(x =>
            (string.IsNullOrEmpty(productParams.Search) 
            || x.Name.ToLower().Contains(productParams.Search.ToLower()) 
            || (x.Description != null && x.Description.ToLower().Contains(productParams.Search.ToLower()))) &&
            (!productParams.CategoryId.HasValue || x.CategoryId == productParams.CategoryId) &&
            (!productParams.MinPrice.HasValue || x.BasePrice >= productParams.MinPrice) &&
            (!productParams.MaxPrice.HasValue || x.BasePrice <= productParams.MaxPrice))
    {
        
    }

    
}