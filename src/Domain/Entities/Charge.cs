using ToursApp.Domain.Common;

namespace ToursApp.Domain.Entities;

public class Charge : AuditableEntity
{
    public Guid Id { get; set; }
    public string StripeChargeId { get; set; } = string.Empty;
    public string ChargeId { get;  set; } = string.Empty;
    public string StripePaymentIntentId { get; set; } = string.Empty;// Stripe Charge ID
    public decimal Amount { get;  set; }           // In dollars (not cents)
    public decimal AmountRefunded { get; private set; }
    public ChargeStatus Status { get;  set; }
    public DateTime? RefundedAt { get; private set; }

    public bool Captured { get; private set; }
    public DateTime? CapturedAt { get; private set; }
    public string Currency { get; set; } = string.Empty;      // e.g. "usd"
    public string PaymentMethodType { get; set; } = string.Empty;

    public virtual Tour Tour {get;set;} = null!;
    public virtual Payment? Payment {get; set;}
    public Guid PaymentId { get; set; }  // Foreign key to Payment
    public Charge(string chargeId, decimal amount, string currency, string createdBy)
        : base(createdBy) // This sets the required CreatedBy field
    {
        ChargeId = chargeId;
        Amount = amount;
        Currency = currency;
    }

    public Charge() : base("TEMPORARY") { }


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