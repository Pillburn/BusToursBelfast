using ToursApp.Domain.Common;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;

namespace ToursApp.Domain.Entities;

public class PaymentIntent : BaseEntity
{
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

    public ICollection<Charge> Charges { get; private set; } = new List<Charge>();
    public PaymentIntent(
        string stripePaymentIntentId,
        decimal amount,
        string currency,
        string clientSecret,
        string createdBy,
        PaymentIntentStatus status) : base(createdBy)
    {
        StripePaymentIntentId = stripePaymentIntentId;
        Amount = amount;
        Currency = currency;
        ClientSecret = clientSecret;
        Status = status;
    }
    //Simplified Constructor    
    public PaymentIntent(
        string stripePaymentIntentId,
        decimal amount,
        string currency,
        string clientSecret,
        string createdBy)
        : this(stripePaymentIntentId, amount, currency, clientSecret, createdBy,
              PaymentIntentStatus.RequiresPaymentMethod)
    {
    }
    //EFCORE Constructor
    private PaymentIntent() : base("temp_creator") { }
    // Domain methods

    public void UpdateStatus(PaymentIntentStatus newStatus)
    {
        Status = newStatus;
    }
    public void AddCharge(string chargeId, decimal amount, string currency)
    {
        if (string.IsNullOrEmpty(this.CreatedBy))
            throw new InvalidOperationException("Creator must be set before adding changes.");

        Charges.Add(new Charge(
            chargeId,
            amount,
            currency,
            this.CreatedBy));  // Reuse the payment intent's creator

        Status = PaymentIntentStatus.Succeeded;
    }
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

    public void SetApplicationReference(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Ref cannot be empty");
        ApplicationReference = reference;
    }

    // Additional state transition methods as needed...
}