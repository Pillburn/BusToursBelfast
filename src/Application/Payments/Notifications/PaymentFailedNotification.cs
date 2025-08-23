using MediatR;

public record PaymentFailedNotification(
    Guid PaymentId,
    string StripePaymentIntentId,
    string FailureReason,
    string? ErrorCode = null) : INotification;
