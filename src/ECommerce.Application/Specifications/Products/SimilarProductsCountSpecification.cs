using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Product;
using ECommerce.Domain.Enums.Inventory;


namespace ECommerce.Application.Specifications.Products;

public class SimilarProductsCountSpecification : BaseSpecification<Product, Guid>
{

    public SimilarProductsCountSpecification()
    {
        
    }
    public SimilarProductsCountSpecification(Product product)
        : base(x =>
            x.Id != product.Id &&
            x.CategoryId == product.CategoryId &&
            x.BrandId == product.BrandId &&
            x.BasePrice >= product.BasePrice * 0.8m &&
            x.BasePrice <= product.BasePrice * 1.2m &&
            x.StockStatus == StockStatus.InStock &&
            x.AverageRating >= product.AverageRating - 1 &&
            x.AverageRating <= product.AverageRating + 1 &&
            x.DiscountPercentage >= product.DiscountPercentage - 10 &&
            x.DiscountPercentage <= product.DiscountPercentage + 10 &&
            x.Status == ProductStatus.Published)
    {
        
    }

    
}