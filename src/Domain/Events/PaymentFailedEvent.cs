// Domain/Events/PaymentRefundedEvent.cs
using ToursApp.Domain.Common;
using ToursApp.Domain.Entities;

namespace ToursApp.Domain.Events;
public class PaymentFailedEvent : DomainEvent
{
    public PaymentFailedEvent(Payment payment)
    {
        Payment = payment;
    }

    public Payment Payment { get; }
}