namespace ECommerce.Domain.Entities.Notifications;

public class BrandSubscription : BaseEntity<Guid>
{
    public string UserId { get; set; } = null!;

    public Guid BrandId { get; set; }

    public User User { get; set; } = null!;

    public Brand Brand { get; set; } = null!;
}