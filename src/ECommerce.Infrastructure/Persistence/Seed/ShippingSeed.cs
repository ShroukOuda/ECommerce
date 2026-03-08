using ECommerce.Core.Entities.Shipping;
using ECommerce.Core.Enums.Shipping;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ShippingSeed
{
    private static readonly DateTime BaseDate = new(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly string[] Carriers = ["DHL", "FedEx", "Aramex", "Egypt Post", "UPS"];

    public static void SeedShippings(ModelBuilder modelBuilder)
    {
        var shippings = new List<Shipping>();
        int shippingId = 1;

        // Shipping for orders that have been shipped or delivered (orders 1-375 per OrderSeed)
        // 1-200 Delivered, 201-300 Processing (some shipped), 301-375 Shipped
        for (int orderId = 1; orderId <= 375; orderId++)
        {
            var userIndex = ((orderId - 1) % 180) + 16;
            var orderDate = BaseDate.AddDays((orderId * 33) % 540).AddHours((orderId * 7) % 24);

            // Calculate user's first address ID (same logic as OrderSeed)
            int addressStartId = 1;
            for (int u = 1; u < userIndex; u++)
            {
                addressStartId += (u % 3) + 1;
            }

            ShippingStatus status;
            DateTime? shippedDate;
            DateTime? deliveredDate;

            if (orderId <= 200) // Delivered
            {
                status = ShippingStatus.Delivered;
                shippedDate = orderDate.AddDays(1);
                deliveredDate = orderDate.AddDays(3 + (orderId % 5));
            }
            else if (orderId <= 300) // Processing - label created
            {
                status = ShippingStatus.LabelCreated;
                shippedDate = null;
                deliveredDate = null;
            }
            else // Shipped / In Transit
            {
                status = orderId % 3 == 0 ? ShippingStatus.OutForDelivery : ShippingStatus.InTransit;
                shippedDate = orderDate.AddDays(1);
                deliveredDate = null;
            }

            var methodIndex = orderId % 5;
            ShippingMethod method = methodIndex switch
            {
                0 => ShippingMethod.Standard,
                1 => ShippingMethod.Express,
                2 => ShippingMethod.Standard,
                3 => ShippingMethod.International,
                _ => ShippingMethod.Free
            };

            decimal cost = method switch
            {
                ShippingMethod.Free => 0m,
                ShippingMethod.Standard => 9.99m,
                ShippingMethod.Express => 19.99m,
                ShippingMethod.Overnight => 29.99m,
                ShippingMethod.International => 39.99m,
                _ => 9.99m
            };

            var carrier = Carriers[orderId % 5];

            shippings.Add(new Shipping
            {
                Id = shippingId++,
                OrderId = orderId,
                AddressId = addressStartId,
                TrackingNumber = $"{carrier.Replace(" ", "").ToUpper()}-{orderId:D8}",
                Method = method,
                Cost = cost,
                ShippedDate = shippedDate,
                DeliveredDate = deliveredDate,
                Status = status,
                CreatedAt = orderDate,
                UpdatedAt = deliveredDate ?? shippedDate ?? orderDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<Shipping>().HasData(shippings.ToArray());
    }
}
