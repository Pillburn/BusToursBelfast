// Application/Common/Interfaces/IStripeGateway.cs
using Stripe;

public interface IStripeGate
{
    Task<Event> ConstructEventAsync(string json, string stripeSignature);
    Task<Stripe.PaymentIntent> GetPaymentIntentAsync(string paymentIntentId);
    Task<Refund> CreateRefundAsync(string chargeId, decimal amount, string reason);
}