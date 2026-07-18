using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;


namespace ECommerce.Application.Specifications.Products;

public class ProductSpecification : BaseSpecification<Product, Guid>
{
    public ProductSpecification(ProductSpecParams productParams)
        : base(x =>
            (string.IsNullOrEmpty(productParams.Search) 
            || x.Name.ToLower().Contains(productParams.Search) 
            || (x.Description != null && x.Description.ToLower().Contains(productParams.Search))) &&
            (!productParams.CategoryId.HasValue || x.CategoryId == productParams.CategoryId) &&
            (!productParams.BrandId.HasValue || x. BrandId == productParams.BrandId) &&
            (!productParams.MinPrice.HasValue || x.BasePrice >= productParams.MinPrice) &&
            (!productParams.MaxPrice.HasValue || x.BasePrice <= productParams.MaxPrice) &&
            (!productParams.IsFeatured.HasValue || x.IsFeatured == productParams.IsFeatured) &&
            (!productParams.IsHotDeal.HasValue || x.IsHotDeal == productParams.IsHotDeal) &&
            (!productParams.IsNewArrival.HasValue || x.IsNewArrival == productParams.IsNewArrival))
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
            case ProductSortBy.Rating:
                AddOrderByDescending(p => p.AverageRating);
                break;
            case ProductSortBy.BestSeller:
                AddOrderByDescending(p => p.TotalSales);
                break;
            case ProductSortBy.Newest:
                AddOrderByDescending(p => p.CreatedAt);
                break;
            case ProductSortBy.Oldest:
                AddOrderBy(p => p.CreatedAt);   
                break;
            default:
                AddOrderBy(p => p.CreatedAt);
                break;
        } 

        AddInclude(p => p.Category);
        AddInclude(p => p.Brand);
        AddInclude(p => p.ProductImages);
        ApplyPaging(productParams.PageSize, productParams.PageNumber);

        AsNoTracking(); 

    }

    public ProductSpecification(Guid productId) : base(p => p.Id == productId)
    {
        AsNoTracking();
    }

    
}