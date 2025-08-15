public class StripeEventMapper
{
    public PaymentEventType MapToDomainEvent(string stripeEventType)
    {
        return stripeEventType switch
        {
            Events.PaymentIntentSucceeded => PaymentEventType.PaymentSucceeded,
            Events.PaymentIntentPaymentFailed => PaymentEventType.PaymentFailed,
            Events.ChargeRefunded => PaymentEventType.ChargeRefunded,
            Events.PaymentIntentCanceled => PaymentEventType.PaymentCanceled,
            _ => throw new NotSupportedException($"Event {stripeEventType} not supported")
        };
    }
}