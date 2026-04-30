namespace ECommerce.Domain.Common;

public class BaseImage : BaseEntity<Guid>
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now;
    public int SortOrder { get; set; } = 0;
    public bool IsMain { get; set; } = false;
}