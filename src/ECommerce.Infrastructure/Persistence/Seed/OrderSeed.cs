using ECommerce.Core.Entities.Order;
using ECommerce.Core.Enums.Order;
using ECommerce.Core.Enums.Payment;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class OrderSeed
{
    private static readonly DateTime BaseDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Deterministic product base prices for order item computation
    private static readonly decimal[] ProductPrices =
    {
        0, // index 0 unused
        1299.99m, 799.99m, 1199.99m, 799.99m, 449.99m, 349.99m, 899.99m, 299.99m, 949.99m, 799.99m,
        999.99m, 499.99m, 699.99m, 599.99m, 129.99m, 179.99m, 169.99m, 249.99m, 119.99m, 139.99m,
        1999.99m, 1299.99m, 1499.99m, 699.99m, 1399.99m, 549.99m, 1799.99m, 599.99m, 1699.99m, 449.99m,
        2199.99m, 499.99m, 1099.99m, 1599.99m, 1449.99m, 1099.99m, 599.99m, 499.99m, 1199.99m, 449.99m,
        399.99m, 149.99m, 499.99m, 249.99m, 169.99m, 249.99m, 349.99m, 299.99m, 279.99m, 99.99m,
        79.99m, 179.99m, 59.99m, 1299.99m, 1499.99m, 2199.99m, 349.99m, 499.99m, 379.99m, 1299.99m,
        899.99m, 2499.99m, 2499.99m, 2499.99m, 1199.99m, 399.99m, 299.99m, 759.99m, 1699.99m, 499.99m,
        499.99m, 299.99m, 349.99m, 69.99m, 59.99m, 79.99m, 35.99m, 65.99m, 99.99m, 39.99m,
    };

    private static readonly string[] ProductNames =
    {
        "", 
        "iPhone 15 Pro Max", "iPhone 15", "Samsung Galaxy S24 Ultra", "Samsung Galaxy S24", "Samsung Galaxy A55",
        "Samsung Galaxy A35", "Xiaomi 14 Pro", "Xiaomi Redmi Note 13 Pro", "Huawei P60 Pro", "OnePlus 12",
        "Google Pixel 8 Pro", "Google Pixel 8a", "iPhone 14", "Samsung Galaxy S23 FE", "Xiaomi Redmi 12C",
        "Samsung Galaxy M14", "Realme C55", "OPPO A78", "Tecno Spark 20", "Infinix Hot 40",
        "MacBook Pro 14\" M3 Pro", "MacBook Air 15\" M3", "Dell XPS 15", "Dell Inspiron 15", "HP Spectre x360",
        "HP Pavilion 15", "Lenovo ThinkPad X1 Carbon", "Lenovo IdeaPad Slim 5", "Asus ROG Strix G16", "Asus VivoBook 15",
        "Acer Predator Helios 16", "Acer Aspire 5", "Microsoft Surface Pro 10", "Huawei MateBook X Pro", "Samsung Galaxy Book4 Pro",
        "iPad Pro 12.9\" M4", "iPad Air 11\" M2", "iPad mini 7", "Samsung Galaxy Tab S9 Ultra", "Samsung Galaxy Tab S9 FE",
        "Xiaomi Pad 6 Pro", "Amazon Fire HD 10", "Lenovo Tab P12 Pro", "AirPods Pro 2nd Gen", "AirPods 3rd Gen",
        "Samsung Galaxy Buds3 Pro", "Sony WH-1000XM5", "Sony WF-1000XM5", "Bose QuietComfort 45", "JBL Tune 770NC",
        "Anker Soundcore Liberty 4", "JBL Charge 5", "Sony SRS-XB100", "Samsung QLED 65\" Q80C", "LG OLED C3 55\"",
        "Sony Bravia XR 65\" OLED", "Xiaomi Smart TV A2 43\"", "TCL 55\" 4K QLED", "Hisense 50\" 4K UHD", "Samsung Frame TV 55\"",
        "LG NanoCell 65\"", "Sony Alpha A7 IV", "Canon EOS R6 Mark II", "Nikon Z6 III", "Canon EOS 90D",
        "GoPro HERO12 Black", "DJI Osmo Action 4", "DJI Mini 4 Pro", "Fujifilm X-T5", "PlayStation 5",
        "Xbox Series X", "Xbox Series S", "Nintendo Switch OLED", "DualSense Controller PS5", "Xbox Wireless Controller",
        "Razer Kraken Gaming Headset", "Anker 65W USB-C Charger", "Anker PowerCore 26800", "Logitech MX Master 3S Mouse", "Apple MagSafe Charger",
    };

    public static void SeedOrders(ModelBuilder modelBuilder)
    {
        var orders = new List<Order>();
        var orderItems = new List<OrderItem>();
        var orderItemOptions = new List<OrderItemOption>();
        var orderStatusHistories = new List<OrderStatusHistory>();

        int orderItemId = 1;
        int orderItemOptionId = 1;
        int statusHistoryId = 1;

        // Active coupon IDs (1-10) for 30% of orders
        int[] activeCouponIds = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        for (int orderId = 1; orderId <= 500; orderId++)
        {
            // Assign to user deterministically (spread across 200 users, 0-8 orders each)
            var userIndex = ((orderId - 1) % 185) + 16; // customers are users 16-200
            if (orderId <= 10) userIndex = ((orderId - 1) % 5) + 1; // first 10 orders from admins
            else if (orderId <= 25) userIndex = ((orderId - 11) % 10) + 6; // next 15 from staff
            var userId = UserSeed.GetUserId(userIndex);

            // Order date spread over 18 months
            var dayOffset = (orderId * 33) % 548; // ~18 months of days
            var hourOffset = (orderId * 7) % 24;
            var orderDate = BaseDate.AddDays(-548 + dayOffset).AddHours(hourOffset);

            // Determine status distribution
            OrderStatus status;
            if (orderId % 100 < 40) status = OrderStatus.Delivered;
            else if (orderId % 100 < 60) status = OrderStatus.Processing;
            else if (orderId % 100 < 75) status = OrderStatus.Shipped;
            else if (orderId % 100 < 85) status = OrderStatus.Pending;
            else if (orderId % 100 < 93) status = OrderStatus.Cancelled;
            else status = OrderStatus.Refunded;

            // Number of items per order (1-5)
            var itemCount = (orderId % 5) + 1;
            var subTotal = 0m;
            var couponId = (orderId % 100 < 30) ? activeCouponIds[(orderId - 1) % activeCouponIds.Length] : (int?)null;

            // Addresses from user's addresses
            // Users have (userIndex % 3) + 1 addresses. 
            // We calculate the address ID range for the user
            int addressStartId = 0;
            for (int u = 1; u < userIndex; u++)
                addressStartId += (u % 3) + 1;
            addressStartId++; // 1-based
            var userAddressCount = (userIndex % 3) + 1;
            var shippingAddressId = addressStartId;
            var billingAddressId = userAddressCount > 1 ? addressStartId + 1 : addressStartId;

            // Generate items
            for (int item = 0; item < itemCount; item++)
            {
                var productId = ((orderId * 3 + item * 7) % 80) + 1;
                var quantity = ((orderId + item) % 3) + 1;
                var unitPrice = ProductPrices[productId];
                var totalPrice = unitPrice * quantity;
                subTotal += totalPrice;

                // Variant: we'll use variant ID 1-based, deterministic
                var variantId = ((productId - 1) * 3 + 1); // first variant of each product approximately
                if (variantId > 500) variantId = 1; // safety cap

                orderItems.Add(new OrderItem
                {
                    Id = orderItemId,
                    OrderId = orderId,
                    ProductId = productId,
                    ProductVariantId = variantId,
                    ProductName = ProductNames[productId],
                    VariantName = "Default",
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    CreatedAt = orderDate,
                    UpdatedAt = orderDate,
                    IsDeleted = false
                });

                // Add one order item option per item
                orderItemOptions.Add(new OrderItemOption
                {
                    Id = orderItemOptionId++,
                    OrderItemId = orderItemId,
                    OptionName = "Color",
                    OptionValue = item % 3 == 0 ? "Black" : (item % 3 == 1 ? "White" : "Silver"),
                    PriceAdjustment = 0,
                    CreatedAt = orderDate,
                    UpdatedAt = orderDate,
                    IsDeleted = false
                });

                orderItemId++;
            }

            // Compute financials
            var discountAmount = 0m;
            if (couponId.HasValue)
            {
                // Simplified: apply 10-20% depending on coupon
                var discountPct = couponId.Value <= 5 ? 0.15m : 0.10m;
                discountAmount = Math.Round(subTotal * discountPct, 2);
                if (discountAmount > 50) discountAmount = 50; // cap
            }

            var shippingCost = subTotal > 500 ? 0m : (orderId % 3 == 0 ? 15.99m : (orderId % 3 == 1 ? 9.99m : 25.99m));
            var taxAmount = Math.Round(subTotal * 0.14m, 2); // 14% tax
            var totalAmount = subTotal - discountAmount + shippingCost + taxAmount;

            var orderNumber = $"ORD-{orderDate:yyyyMMdd}-{orderId:D5}";

            orders.Add(new Order
            {
                Id = orderId,
                UserId = userId,
                OrderNumber = orderNumber,
                OrderType = OrderType.Standard,
                OrderStatus = status,
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Currency = "USD",
                ShippingAddressId = shippingAddressId,
                BillingAddressId = billingAddressId,
                CreatedAt = orderDate,
                UpdatedAt = orderDate.AddDays(1),
                IsDeleted = false
            });

            // ----- Order Status History -----
            // Always starts with Pending
            orderStatusHistories.Add(new OrderStatusHistory
            {
                Id = statusHistoryId++,
                OrderId = orderId,
                OrderStatus = OrderStatus.Pending,
                CreatedAt = orderDate,
                UpdatedAt = orderDate,
                IsDeleted = false
            });

            if (status != OrderStatus.Pending)
            {
                orderStatusHistories.Add(new OrderStatusHistory
                {
                    Id = statusHistoryId++,
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Confirmed,
                    CreatedAt = orderDate.AddHours(2),
                    UpdatedAt = orderDate.AddHours(2),
                    IsDeleted = false
                });
            }

            if (status == OrderStatus.Processing || status == OrderStatus.Shipped ||
                status == OrderStatus.Delivered || status == OrderStatus.Refunded)
            {
                orderStatusHistories.Add(new OrderStatusHistory
                {
                    Id = statusHistoryId++,
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Processing,
                    CreatedAt = orderDate.AddHours(12),
                    UpdatedAt = orderDate.AddHours(12),
                    IsDeleted = false
                });
            }

            if (status == OrderStatus.Shipped || status == OrderStatus.Delivered)
            {
                orderStatusHistories.Add(new OrderStatusHistory
                {
                    Id = statusHistoryId++,
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Shipped,
                    CreatedAt = orderDate.AddDays(2),
                    UpdatedAt = orderDate.AddDays(2),
                    IsDeleted = false
                });
            }

            if (status == OrderStatus.Delivered)
            {
                orderStatusHistories.Add(new OrderStatusHistory
                {
                    Id = statusHistoryId++,
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Delivered,
                    CreatedAt = orderDate.AddDays(5),
                    UpdatedAt = orderDate.AddDays(5),
                    IsDeleted = false
                });
            }

            if (status == OrderStatus.Cancelled)
            {
                orderStatusHistories.Add(new OrderStatusHistory
                {
                    Id = statusHistoryId++,
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Cancelled,
                    CreatedAt = orderDate.AddHours(6),
                    UpdatedAt = orderDate.AddHours(6),
                    IsDeleted = false
                });
            }

            if (status == OrderStatus.Refunded)
            {
                orderStatusHistories.Add(new OrderStatusHistory
                {
                    Id = statusHistoryId++,
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Refunded,
                    CreatedAt = orderDate.AddDays(10),
                    UpdatedAt = orderDate.AddDays(10),
                    IsDeleted = false
                });
            }
        }

        modelBuilder.Entity<Order>().HasData(orders.ToArray());
        modelBuilder.Entity<OrderItem>().HasData(orderItems.ToArray());
        modelBuilder.Entity<OrderItemOption>().HasData(orderItemOptions.ToArray());
        modelBuilder.Entity<OrderStatusHistory>().HasData(orderStatusHistories.ToArray());
    }
}
