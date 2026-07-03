using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;
using ECommerce.Application.DTO.Product;

namespace ECommerce.Application.Specifications.Products;

public class ProductSpecification : BaseSpecification<Product, Guid>
{
    public ProductSpecification(ProductSpecParams productParams)
        : base(x =>
            (string.IsNullOrEmpty(productParams.Search) 
            || x.Name.ToLower().Contains(productParams.Search.ToLower()) 
            || (x.Description != null && x.Description.ToLower().Contains(productParams.Search.ToLower()))) &&
            (!productParams.CategoryId.HasValue || x.CategoryId == productParams.CategoryId) &&
            (!productParams.MinPrice.HasValue || x.BasePrice >= productParams.MinPrice) &&
            (!productParams.MaxPrice.HasValue || x.BasePrice <= productParams.MaxPrice))
    {
        switch (productParams.SortBy)
        {
            case ProductSortBy.NameAsc:
                AddOrderBy(p => p.Name);
                break;
            case ProductSortBy.NameDesc:
                AddOrderByDescending(p => p.Name);
                break;
            case ProductSortBy.PriceAsc:
                AddOrderBy(p => p.BasePrice);
                break;
            case ProductSortBy.PriceDesc:
                AddOrderByDescending(p => p.BasePrice);
                break;
            case ProductSortBy.Newest:
                AddOrderByDescending(p => p.CreatedAt);
                break;
            case ProductSortBy.Oldest:
                AddOrderBy(p => p.CreatedAt);   
                break;
        } 

        AddInclude(p => p.Category);
        AddInclude(p => p.ProductImages);
        
        ApplyPaging(productParams.PageSize, productParams.PageNumber);

        AsNoTracking(); 

    }

    
}