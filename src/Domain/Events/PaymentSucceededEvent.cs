// Domain/Events/PaymentRefundedEvent.cs
using ToursApp.Domain.Common;
using ToursApp.Domain.Entities;

namespace ToursApp.Domain.Events;
public class PaymentSucceededEvent : DomainEvent
{
    public PaymentSucceededEvent(Payment payment)
    {
        Payment = payment;
    }

    public Payment Payment { get; }
}