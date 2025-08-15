// Application/Constants/StripeWebhookEvents.cs
public static class StripeWebhookEvents
{
    public const string PaymentIntentSucceeded = "payment_intent.succeeded";
    public const string ChargeRefunded = "charge.refunded";
    public const string PaymentIntentFailed = "payment_intent.payment_failed";
    // Add others as needed
}