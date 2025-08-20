using Stripe;
public class StripeEventMapper
{
    public PaymentEventType MapToDomainEvent(string stripeEventType)
    {
        return stripeEventType switch
        {
            "payment_intent.succeeded" => PaymentEventType.PaymentSucceeded,
            "payment_intent.payment_failed" => PaymentEventType.PaymentFailed,
            "charge.refunded" => PaymentEventType.ChargeRefunded,
            "payment_intent.canceled" => PaymentEventType.PaymentCanceled,
            _ => throw new NotSupportedException($"Event {stripeEventType} not supported")
        };
    }
}