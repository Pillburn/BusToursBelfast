using ToursApp.Domain.Common;

public class Charge : AuditableEntity
{
    public Guid Id { get; private set; }
    
    // Stripe's charge ID
    public string StripeChargeId { get; private set; }
    
    // Reference to PaymentIntent
    public string StripePaymentIntentId { get; private set; }
    
    public decimal Amount { get; private set; }
    public decimal AmountRefunded { get; private set; }
    public string Currency { get; private set; } = "gbp";
    
    public ChargeStatus Status { get; private set; }
    public bool Captured { get; private set; }
    public bool Refunded => AmountRefunded > 0;
    
    // Payment method details
    public string PaymentMethodId { get; private set; }
    public string PaymentMethodType { get; private set; }
    
    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? CapturedAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    
    // Relationships
    public Guid? BookingId { get; private set; } // Your domain reference
    
    // Private constructor for EF Core
    private Charge() { }

    public Charge(
        string stripeChargeId,
        string stripePaymentIntentId,
        decimal amount,
        string currency,
        string paymentMethodId,
        string paymentMethodType,
        Guid? bookingId = null)
    {
        StripeChargeId = stripeChargeId;
        StripePaymentIntentId = stripePaymentIntentId;
        Amount = amount;
        Currency = currency;
        PaymentMethodId = paymentMethodId;
        PaymentMethodType = paymentMethodType;
        BookingId = bookingId;
        Status = ChargeStatus.Pending;
        Captured = false;
        CreatedAt = DateTime.UtcNow;
    }

    // Domain methods
    public void MarkAsCaptured(DateTime capturedAt)
    {
        if (Captured)
            throw new InvalidOperationException("Charge already captured");
            
        Captured = true;
        Status = ChargeStatus.Succeeded;
        CapturedAt = capturedAt;
    }

    public void Refund(decimal amount, DateTime refundedAt)
    {
        if (amount <= 0 || amount > (Amount - AmountRefunded))
            throw new ArgumentException("Invalid refund amount");
        
        AmountRefunded += amount;
        RefundedAt = refundedAt;
        
        Status = AmountRefunded == Amount 
            ? ChargeStatus.FullyRefunded 
            : ChargeStatus.PartiallyRefunded;
    }

    public void MarkAsFailed(string failureReason)
    {
        Status = ChargeStatus.Failed;
        // Additional failure handling logic...
    }
}