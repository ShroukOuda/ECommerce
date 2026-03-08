using ECommerce.Core.Entities.Inventory;
using ECommerce.Core.Enums.Inventory;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class InventoryHistorySeed
{
    private static readonly DateTime BaseDate = new(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedInventoryHistory(ModelBuilder modelBuilder)
    {
        var records = new List<InventoryHistory>();
        int id = 1;

        // Initial stock entries for all 80 products
        for (int productId = 1; productId <= 80; productId++)
        {
            int initialQty = 100 + (productId * 7) % 400; // 100-500

            records.Add(new InventoryHistory
            {
                Id = id++,
                ProductId = productId,
                ProductVariantId = null,
                UserId = UserSeed.GetUserId(1), // Admin user
                QuantityChange = initialQty,
                NewQuantity = initialQty,
                ChangeType = InventoryChangeType.Restock,
                ReferencedId = null,
                ReferencedType = "InitialStock",
                Notes = $"Initial inventory load for product #{productId}",
                CreatedAt = BaseDate,
                UpdatedAt = BaseDate,
                IsDeleted = false
            });
        }

        // Restock events (80 records, one per product at a later date)
        for (int productId = 1; productId <= 80; productId++)
        {
            int initialQty = 100 + (productId * 7) % 400;
            int restockQty = 50 + (productId * 3) % 200;
            int newQty = initialQty + restockQty;
            var restockDate = BaseDate.AddDays(90 + productId);

            records.Add(new InventoryHistory
            {
                Id = id++,
                ProductId = productId,
                ProductVariantId = null,
                UserId = UserSeed.GetUserId(2), // Admin user
                QuantityChange = restockQty,
                NewQuantity = newQty,
                ChangeType = InventoryChangeType.Restock,
                ReferencedId = $"PO-{productId:D4}",
                ReferencedType = "PurchaseOrder",
                Notes = $"Restock from supplier for product #{productId}",
                CreatedAt = restockDate,
                UpdatedAt = restockDate,
                IsDeleted = false
            });
        }

        // Purchase/sale deductions for first 100 orders (simulated)
        for (int i = 1; i <= 100; i++)
        {
            var productId = ((i * 3) % 80) + 1;
            int initialQty = 100 + (productId * 7) % 400;
            int restockQty = 50 + (productId * 3) % 200;
            var totalBefore = initialQty + restockQty;
            var soldQty = (i % 5) + 1;
            var saleDate = BaseDate.AddDays(120 + i);

            records.Add(new InventoryHistory
            {
                Id = id++,
                ProductId = productId,
                ProductVariantId = null,
                UserId = null,
                QuantityChange = -soldQty,
                NewQuantity = totalBefore - soldQty,
                ChangeType = InventoryChangeType.Purchase,
                ReferencedId = i.ToString(),
                ReferencedType = "Order",
                Notes = $"Sold {soldQty} unit(s) via order #{i}",
                CreatedAt = saleDate,
                UpdatedAt = saleDate,
                IsDeleted = false
            });
        }

        // Return stock additions (20 records, matching first 20 returns)
        for (int returnId = 1; returnId <= 20; returnId++)
        {
            var orderId = returnId * 4;
            var productId = ((orderId * 3) % 80) + 1;
            var returnDate = BaseDate.AddDays(200 + returnId * 3);

            records.Add(new InventoryHistory
            {
                Id = id++,
                ProductId = productId,
                ProductVariantId = null,
                UserId = UserSeed.GetUserId(3), // Staff user
                QuantityChange = 1,
                NewQuantity = 200, // approximate
                ChangeType = InventoryChangeType.Return,
                ReferencedId = returnId.ToString(),
                ReferencedType = "ReturnRequest",
                Notes = $"Item returned and restocked from return #{returnId}",
                CreatedAt = returnDate,
                UpdatedAt = returnDate,
                IsDeleted = false
            });
        }

        // Damage/adjustment entries (15 records)
        for (int i = 1; i <= 10; i++)
        {
            var productId = i * 8;
            if (productId > 80) productId = 80;
            var adjDate = BaseDate.AddDays(150 + i * 5);

            records.Add(new InventoryHistory
            {
                Id = id++,
                ProductId = productId,
                ProductVariantId = null,
                UserId = UserSeed.GetUserId(6), // Staff user
                QuantityChange = -((i % 3) + 1),
                NewQuantity = 180,
                ChangeType = InventoryChangeType.Damage,
                ReferencedId = null,
                ReferencedType = "DamageReport",
                Notes = $"Damaged unit(s) removed from inventory for product #{productId}",
                CreatedAt = adjDate,
                UpdatedAt = adjDate,
                IsDeleted = false
            });
        }

        for (int i = 1; i <= 5; i++)
        {
            var productId = i * 15;
            if (productId > 80) productId = 75;
            var adjDate = BaseDate.AddDays(180 + i * 3);

            records.Add(new InventoryHistory
            {
                Id = id++,
                ProductId = productId,
                ProductVariantId = null,
                UserId = UserSeed.GetUserId(7), // Staff user
                QuantityChange = (i % 2 == 0) ? 5 : -3,
                NewQuantity = (i % 2 == 0) ? 205 : 177,
                ChangeType = InventoryChangeType.Adjustment,
                ReferencedId = null,
                ReferencedType = "ManualAdjustment",
                Notes = $"Inventory count adjustment for product #{productId}",
                CreatedAt = adjDate,
                UpdatedAt = adjDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<InventoryHistory>().HasData(records.ToArray());
    }
}
