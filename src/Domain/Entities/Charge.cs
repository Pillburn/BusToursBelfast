using ToursApp.Domain.Common;

namespace ToursApp.Domain.Entities;

public class Charge : AuditableEntity
{
    public string StripeChargeId { get; private set; } 
    public string ChargeId { get; private set; }
    public string StripePaymentIntentId { get; set; }               // Stripe Charge ID
    public decimal Amount { get; private set; }           // In dollars (not cents)
    public decimal AmountRefunded { get; private set; }
    public ChargeStatus Status { get; private set; }
    public DateTime? RefundedAt { get; private set; }

    public bool Captured { get; private set; }
    public DateTime? CapturedAt { get; private set; }
    public string Currency { get; private set; }          // e.g. "usd"
    public string PaymentMethodType { get; set; }
    public Charge(string chargeId, decimal amount, string currency, string createdBy)
        : base(createdBy) // This sets the required CreatedBy field
    {
        ChargeId = chargeId;
        Amount = amount;
        Currency = currency;
    }

    private Charge() : base("TEMPORARY") { }


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