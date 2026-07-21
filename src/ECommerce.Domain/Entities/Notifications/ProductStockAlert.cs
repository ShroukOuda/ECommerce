namespace ECommerce.Domain.Entities.Notifications;

public class ProductStockAlert : BaseEntity<Guid>
{
    public string UserId { get; set; } = null!;

    public Guid ProductId { get; set; }

    public bool IsNotified { get; set; }

    public User User { get; set; } = null!;

    public Product Product { get; set; } = null!;
}