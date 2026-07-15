using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Brands;
using ECommerce.Domain.Enums.Media;

namespace ECommerce.Application.Specifications.Brands;

public class BrandLogoSpecification : BaseSpecification<BrandLogo, Guid>
{
    public BrandLogoSpecification(Guid brandId)
        : base(l => l.BrandId == brandId)
    {
        AddOrderBy(l => l.SortOrder);
        AsNoTracking();
    }

    public BrandLogoSpecification(Guid brandId, ImageSubType subType)
        : base(l => l.BrandId == brandId && l.SubType == subType)
    {
        AsNoTracking();
    }

    public BrandLogoSpecification(Guid brandId, Guid logoId)
        : base(l => l.BrandId == brandId && l.Id == logoId)
    {
        AsNoTracking();
    }

    
}