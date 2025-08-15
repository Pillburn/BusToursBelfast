// Domain/Events/PaymentRefundedEvent.cs
using ToursApp.Domain.Entities;

public class PaymentRefundedEvent : DomainEvent
{
    public PaymentRefundedEvent(Payment payment)
    {
        Payment = payment;
    }

    public Payment Payment { get; }
}