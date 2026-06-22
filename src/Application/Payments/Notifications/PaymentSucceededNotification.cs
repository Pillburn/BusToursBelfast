// Application/Notifications/PaymentSucceededNotification.cs
using MediatR;

namespace ToursApp.Application.Payments.Notifications;
public class PaymentSucceededNotification : INotification
{
    public string PaymentIntentId { get; }
    public string ChargeId { get; }
    public string BookingId { get; }
    public string ReferenceNumber { get; }
    public decimal Amount { get; }
    public DateTime PaidAt { get; }
    public string Currency { get; }

    public PaymentSucceededNotification(
        string paymentIntentId,
        string chargeId,
        string bookingId,
        string referenceNumber,
        decimal amount,
        DateTime paidAt,
        string currency)
    {
        PaymentIntentId = paymentIntentId;
        ChargeId = chargeId;
        BookingId = bookingId;
        ReferenceNumber = referenceNumber;
        Amount = amount;
        PaidAt = paidAt;
        Currency = currency;
    }
}