namespace ECommerce.Domain.Entities.Notifications;

public class CategorySubscription : BaseEntity<Guid>
{
    public string UserId { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public User User { get; set; } = null!;

    public Category Category { get; set; } = null!;
}