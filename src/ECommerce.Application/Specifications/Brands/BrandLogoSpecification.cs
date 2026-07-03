using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Brands;

namespace ECommerce.Application.Specifications.Brands;

public class BrandLogoSpecification : BaseSpecification<BrandLogo, Guid>
{
    public BrandLogoSpecification(Guid brandId)
        : base(l => l.BrandId == brandId)
    {
        AddOrderBy(l => l.SortOrder);
        AsNoTracking();
    }

    
}