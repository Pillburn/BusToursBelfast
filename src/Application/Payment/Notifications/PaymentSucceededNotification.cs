// Application/Notifications/PaymentSucceededNotification.cs
using MediatR;

public record PaymentSucceededNotification(
    string PaymentIntentId,
    string ChargeId,
    decimal Amount,
    string Currency) : INotification;