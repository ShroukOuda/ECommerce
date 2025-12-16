namespace ECommerce.Core.Common;

public class BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    public  DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}