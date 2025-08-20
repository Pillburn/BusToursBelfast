using ToursApp.Domain.Common;
using ToursApp.Domain.Enums;
using ToursApp.Domain.Events;


namespace ToursApp.Domain.Entities;

public class Payment : BaseEntity
{
    public required string PaymentIntentId { get; set; } // Stripe PaymentIntent ID
    public decimal Amount { get; set; } // Original payment amount
    public decimal AmountRefunded { get; set; } = 0; // Tracks total refunded amount
    public string Currency { get; set; }

    public Charge? Charge { get; set; }
    public PaymentStatus Status { get; set; } // Using enum instead of string
    public string? ChargeId { get; set; } // Stripe Charge ID
    public string? FailureReason { get; set; }
    public DateTime? LastRefundedAt { get; set; }
    public string? LastRefundId { get; set; } // Most recent Stripe Refund ID
    public DateTime? PaidAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public PaymentIntent? PaymentIntent { get; set; }

    // Navigation properties (if using EF Core relationships)
    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }

    public Payment(string createdBy) : base(createdBy) // Pass to base
    {
        // Initialize default values
        Status = PaymentStatus.Pending;
    }

    // Full constructor
    public Payment(
        decimal amount,
        string currency,
        string createdBy) : base(createdBy)
    {
        Amount = amount;
        Currency = currency;
        Status = PaymentStatus.Pending;
    }

    // EF Core constructor
    protected Payment() : base("system") { } // Temporary value for EF
    public void RecordRefund(decimal amount, DateTime refundedAt, string stripeRefundId)
    {
        AmountRefunded += amount;
        LastRefundedAt = refundedAt;
        LastRefundId = stripeRefundId;

        Status = AmountRefunded >= Amount
            ? PaymentStatus.FullyRefunded
            : PaymentStatus.PartiallyRefunded;

        AddDomainEvent(new PaymentRefundedEvent(this));
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        FailedAt = DateTime.UtcNow;
        AddDomainEvent(new PaymentFailedEvent(this));
    }

    public void MarkAsPaid(string chargeId)
    {
        Status = PaymentStatus.Succeeded;
        ChargeId = chargeId;
        PaidAt = DateTime.UtcNow;
        AddDomainEvent(new PaymentSucceededEvent(this));
    }
}