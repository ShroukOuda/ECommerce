using ECommerce.Core.Entities.Payment;
using ECommerce.Core.Enums.Payment;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class PaymentSeed
{
    private static readonly DateTime BaseDate = new(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedPayments(ModelBuilder modelBuilder)
    {
        var payments = new List<Payment>();

        // One payment per order (500 orders)
        // Order status distribution from OrderSeed:
        //   1-200 Delivered, 201-300 Processing, 301-375 Shipped,
        //   376-425 Pending, 426-465 Cancelled, 466-500 Refunded
        for (int orderId = 1; orderId <= 500; orderId++)
        {
            var userIndex = ((orderId - 1) % 180) + 16; // users 16-195
            var userId = UserSeed.GetUserId(userIndex);
            var orderDate = BaseDate.AddDays((orderId * 33) % 540).AddHours((orderId * 7) % 24);

            PaymentStatus status;
            if (orderId <= 200) status = PaymentStatus.Paid;
            else if (orderId <= 300) status = PaymentStatus.Pending;
            else if (orderId <= 375) status = PaymentStatus.Paid;
            else if (orderId <= 425) status = PaymentStatus.Pending;
            else if (orderId <= 465) status = PaymentStatus.Failed;
            else status = PaymentStatus.Refunded;

            var methodIndex = (orderId % 7);
            PaymentMethod method = methodIndex switch
            {
                0 => PaymentMethod.CreditCard,
                1 => PaymentMethod.DebitCard,
                2 => PaymentMethod.PayPal,
                3 => PaymentMethod.Stripe,
                4 => PaymentMethod.ApplePay,
                5 => PaymentMethod.CashOnDelivery,
                _ => PaymentMethod.BankTransfer
            };

            string gateway = method switch
            {
                PaymentMethod.PayPal => "PayPal",
                PaymentMethod.Stripe => "Stripe",
                PaymentMethod.ApplePay => "Stripe",
                PaymentMethod.GooglePay => "Stripe",
                _ => "Paymob"
            };

            // Approximate order amount
            var baseItemCount = (orderId % 5) + 1;
            decimal approxAmount = baseItemCount * 250m + (orderId % 300);
            var paidAt = status == PaymentStatus.Paid || status == PaymentStatus.Refunded
                ? orderDate.AddMinutes(5)
                : orderDate;

            payments.Add(new Payment
            {
                Id = orderId,
                OrderId = orderId,
                UserId = userId,
                TransactionId = $"TXN-{orderId:D6}",
                Currency = "USD",
                PaymentGateway = gateway,
                GatewayTransactionId = $"GW-{orderId:D8}",
                GatewayResponse = status == PaymentStatus.Paid ? "{\"status\":\"success\"}"
                    : status == PaymentStatus.Failed ? "{\"status\":\"declined\",\"reason\":\"insufficient_funds\"}"
                    : status == PaymentStatus.Refunded ? "{\"status\":\"refunded\"}"
                    : null,
                Status = status,
                Method = method,
                Amount = approxAmount,
                PaidAt = paidAt,
                CreatedAt = orderDate,
                UpdatedAt = paidAt,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<Payment>().HasData(payments.ToArray());
    }
}
