using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Media;

namespace ECommerce.Domain.Entities.Category;

public class CategoryImage : BaseImage
{
    //FK
    public Guid CategoryId { get; set; }   
    public ImageSubType? SubType { get; set; }
    
    //Navigation Properties
    public virtual Category? Category { get; set; }
}