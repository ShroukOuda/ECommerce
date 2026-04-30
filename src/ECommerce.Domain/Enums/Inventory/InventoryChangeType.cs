namespace ECommerce.Domain.Enums.Inventory;

public enum InventoryChangeType
{
    Purchase = 1,
    Restock = 2,
    Adjustment = 3,
    Return = 4,
    Damage = 5,
    Expired = 6,
    Transfer = 7
}