using ToursApp.Domain.Common;
public class PaymentIntent : AuditableEntity
{
    public Guid Id { get; set; }
    
    // Stripe's ID for the PaymentIntent
    public string StripePaymentIntentId { get; private set; }
    
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "gbp";

    // Update PaymentIntent entity to use Money:
    public Money GetMoney() => new(Amount, Currency);
    
    // Enum for status (see below)
    public PaymentIntentStatus Status { get; private set; }
    
    // Your application's reference (e.g. booking ID)
    public string? ApplicationReference { get; private set; }
    
    // Client secret (transient, for frontend use)
    public string? ClientSecret { get; private set; }

    // Payment method details
    public string? PaymentMethodId { get; private set; }
    public DateTime? PaymentCapturedAt { get; private set; }

    private PaymentIntent() { } // For EF Core

    public PaymentIntent(string stripePaymentIntentId, decimal amount, 
                        string currency, string clientSecret)
    {
        StripePaymentIntentId = stripePaymentIntentId;
        Amount = amount;
        Currency = currency;
        ClientSecret = clientSecret;
        Status = PaymentIntentStatus.RequiresPaymentMethod;
    }

    // Domain methods
    public void MarkAsRequiresAction() 
    {
        Status = PaymentIntentStatus.RequiresAction;
    }

    public void CompletePayment(string paymentMethodId, DateTime capturedAt)
    {
        PaymentMethodId = paymentMethodId;
        PaymentCapturedAt = capturedAt;
        Status = PaymentIntentStatus.Succeeded;
    }

    // Additional state transition methods as needed...
}