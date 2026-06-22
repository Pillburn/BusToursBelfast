using Stripe;
using ToursApp.Application.Common.Interfaces;
public class StripeEventMapper : IStripeEventMapper
{
    public PaymentEventType MapToDomainEvent(string stripeEventType)
    {
        return stripeEventType switch
        {
            "payment_intent.succeeded" => PaymentEventType.PaymentSucceeded,
                "payment_intent.payment_failed" => PaymentEventType.PaymentFailed,
                "charge.refunded" => PaymentEventType.ChargeRefunded,
                "payment_intent.canceled" => PaymentEventType.PaymentCanceled,
                "charge.succeeded" => PaymentEventType.ChargeSucceeded,
                "payment_intent.created" => PaymentEventType.PaymentIntentCreated,
                _ => throw new NotSupportedException($"Stripe event type '{stripeEventType}' is not supported")
        };
    }
}