using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductCountSpecification : BaseSpecification<Product, Guid>
{

    public ProductCountSpecification()
    {
        
    }
    public ProductCountSpecification(ProductSpecParams productParams)
        : base(x =>
        (string.IsNullOrEmpty(productParams.Search) 
        || x.Name.ToLower().Contains(productParams.Search) 
        || (x.Description != null && x.Description.ToLower().Contains(productParams.Search))) &&
        (!productParams.CategoryId.HasValue || x.CategoryId == productParams.CategoryId) &&
        (!productParams.BrandId.HasValue || x. BrandId == productParams.BrandId) &&
        (!productParams.MinPrice.HasValue || x.BasePrice >= productParams.MinPrice) &&
        (!productParams.MaxPrice.HasValue || x.BasePrice <= productParams.MaxPrice) &&
        (!productParams.IsFeatured.HasValue || x.IsFeatured == productParams.IsFeatured) &&
        (!productParams.IsBestSeller.HasValue || x.IsBestSeller == productParams.IsBestSeller) &&
        (!productParams.IsHotDeal.HasValue || x.IsHotDeal == productParams.IsHotDeal) &&
        (!productParams.IsNewArrival.HasValue || x.IsNewArrival == productParams.IsNewArrival) &&
        (!productParams.IsTopRated.HasValue || x.IsTopRated == productParams.IsTopRated))
    {
        
    }

    
}