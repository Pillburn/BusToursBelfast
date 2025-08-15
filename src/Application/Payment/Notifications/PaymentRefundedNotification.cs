using MediatR;

public record PaymentRefundedNotification(
    Guid PaymentId,
    string StripeRefundId,
    decimal AmountRefunded,
    string StripeChargeId,
    string Reason) : INotification;
