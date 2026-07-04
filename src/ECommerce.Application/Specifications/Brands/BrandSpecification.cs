using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Brands;

namespace ECommerce.Application.Specifications.Brands;

public class BrandSpecification : BaseSpecification<Brand, Guid>
{
    public BrandSpecification(Guid brandId)
        : base(b => b.Id == brandId)
    {
        AsNoTracking();
    }

    
}